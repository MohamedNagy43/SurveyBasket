using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using OneOf;
using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Api.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    SignInManager<ApplicationUser> signInManager,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor
) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly int _refreshTokenExpirationDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, true);

        if (result.IsLockedOut)
            return Result.Failure<AuthResponse>(UserErrors.UserLockedOut);

        if (result.IsNotAllowed)
            return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);

        if (!result.Succeeded)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);


        var (roles, permissions) = await GetUserRolesAndPermissions(user, cancellationToken);
        var (accesstoken, expiresIn) = await _jwtProvider.GenerateTokenAsync(user, roles, permissions);

        var newRefreshToken = new RefreshToken(_refreshTokenExpirationDays);
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);

        return Result.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, accesstoken, expiresIn, newRefreshToken.Token, newRefreshToken.ExpiresOn));

    }
    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        bool isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (isEmailExist)
            return Result.Failure(UserErrors.DublicatedEmail);

        var newUser = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("Error Code: {Code} Description: {Description}", error.Code, error.Description);
            }
            return Result.Failure(new Error("ServerError", "Something went Wrong", 500));
        }

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        await SendConfirmationEmailAsync(newUser, code);
        return Result.Success();
    }
    public async Task<Result> ResendEmailConfirmationCodeAsync(ResendEmailConfirmationCodeRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        await SendConfirmationEmailAsync(user, code);

        return Result.Success();
    }
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidEmailConfirmationCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        }
        catch
        {
            return Result.Failure(UserErrors.InvalidEmailConfirmationCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return Result.Failure(UserErrors.InvalidEmailConfirmationCode);

        await _userManager.AddToRoleAsync(user, DefaultRoles.Member);

        return Result.Success();
    }
    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        _logger.LogInformation("reset password Code : {code}", code);

        // build email and sent
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        var body = await EmailBodyBuilder.BuildEmailBodyAsync("ForgetPassword", new Dictionary<string, string>
        {
            {"{{name}}",user.FirstName},
            {"{{action_url}}",$"{origin}/auth/reset-password?email={user.Email}&code={code}"} // frontEnd Diriction
        });

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(email, "SurveyBasket: reset your email", body));
        return Result.Success();
    }
    public async Task<Result> ResetPasswordCodeAsync(ResetPasswordRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure(UserErrors.InvalidForgetPasswordCode);

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        }
        catch
        {
            return Result.Failure(UserErrors.InvalidForgetPasswordCode);
        }
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();
        else
            return Result.Failure(UserErrors.InvalidForgetPasswordCode);
    }
    public async Task<OneOf<AuthResponse, Error>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        // Access Token Validation
        string? UserId = _jwtProvider.ValidateToken(token, validateLifeTime: false);
        if (UserId is null)
            return UserErrors.InvalidTokens;

        var user = await _userManager.FindByIdAsync(UserId);
        if (user is null)
            return UserErrors.InvalidTokens;

        if (user.IsDisabled)
            return UserErrors.UserDisabled;

        if (user.LockoutEnd is not null && user.LockoutEnd > DateTime.UtcNow)
            return UserErrors.UserLockedOut;

        // Refresh Token Validation
        RefreshToken? existToken = user.RefreshTokens.SingleOrDefault(refresh => refresh.Token == refreshToken && refresh.IsActive);

        if (existToken is null)
            return UserErrors.InvalidTokens;


        existToken.Revoke();

        // Generate New Access Token and Refresh Token

        var (roles, permissions) = await GetUserRolesAndPermissions(user, cancellationToken);
        var (accesstoken, expiresIn) = await _jwtProvider.GenerateTokenAsync(user, roles, permissions);

        var newRefreshToken = new RefreshToken
        {
            Token = GenerateRefreshToken(),
            ExpiresOn = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
        };

        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, accesstoken, expiresIn,
            newRefreshToken.Token, newRefreshToken.ExpiresOn);

    }
    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        // Access Token Validation
        string? UserId = _jwtProvider.ValidateToken(token, validateLifeTime: true);
        if (UserId is null)
            return Result.Failure(UserErrors.InvalidTokens);

        var user = await _userManager.FindByIdAsync(UserId);
        if (user is null)
            return Result.Failure(UserErrors.InvalidTokens);

        // Refresh Token Validation
        RefreshToken? existToken = user.RefreshTokens.SingleOrDefault(refresh => refresh.Token == refreshToken && refresh.IsActive);

        if (existToken is null)
            return Result.Failure(UserErrors.InvalidTokens);

        existToken.Revoke();
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
    private async Task SendConfirmationEmailAsync(ApplicationUser user, string code)
    {
        // Email Sender
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        string emailBody = await EmailBodyBuilder.BuildEmailBodyAsync("ConfirmationEmail", new Dictionary<string, string>
        {
            {"{{name}}",user.FirstName},
            {"{{action_url}}",$"{origin}/auth/ConfirmationEmail?userId={user.Id}&code={code}"} // frontEnd Diriction
        });

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Confirm email at survey basket", emailBody));
    }
    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var permissions = await (

            from role in _context.Roles
            join claim in _context.RoleClaims
            on role.Id equals claim.RoleId
            where userRoles.Contains(role.Name!) && claim.ClaimType == Permissions.Type
            select claim.ClaimValue

        ).Distinct().ToListAsync(cancellationToken);

        return (userRoles, permissions);
    }

}
