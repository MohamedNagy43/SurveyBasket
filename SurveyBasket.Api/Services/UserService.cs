using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Api.Contracts.Users;
using SurveyBasket.Api.Helpers;
using System.Text;

namespace SurveyBasket.Api.Services;

public class UserService(UserManager<ApplicationUser> userManager,
    ApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender,
    IRoleService roleService) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IRoleService _roleService = roleService;

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await (

            from user in _context.Users
            join userRole in _context.UserRoles
            on user.Id equals userRole.UserId
            join role in _context.Roles
            on userRole.RoleId equals role.Id
            where role.Name != DefaultRoles.Member
            group role by new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.IsDisabled,
            } into g
            select new UserResponse(g.Key.Id, g.Key.FirstName, g.Key.LastName, g.Key.Email, g.Key.IsDisabled, g.Select(x => x.Name!))
        ).ToListAsync(cancellationToken);

        return response;
    }
    public async Task<Result<UserResponse>> GetAsync(string id)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(Errors<ApplicationUser>.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var response = new UserResponse(user.Id, user.FirstName, user.LastName, user.Email!, user.IsDisabled, roles);

        return Result.Success(response);
    }
    public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Validation
        bool isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (isEmailExist)
            return Result.Failure<UserResponse>(UserErrors.DublicatedEmail);

        var allowedRoles = (await _roleService.GetAllAsync(cancellationToken: cancellationToken)).Select(x => x.Name);

        if (request.Roles.Except(allowedRoles).Any())
            return Result.Failure<UserResponse>(RoleErrors.InvaildRoles);

        // Add
        var newUser = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(newUser);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        await _userManager.AddToRolesAsync(newUser, request.Roles);

        // send confirmation email
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        await SendConfirmationEmailAsync(newUser, request.Roles[0], code);

        return Result.Success(new UserResponse(newUser.Id, newUser.FirstName, newUser.LastName, newUser.Email!, newUser.IsDisabled, request.Roles));
    }
    public async Task<Result> ConfirmEmailAndSetPasswordAsync(ConfirmEmailAndSetPasswordRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
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


        result = await _userManager.AddPasswordAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        return Result.Success();
    }
    public async Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        // validation
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(Errors<ApplicationUser>.NotFound);

        bool isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id, cancellationToken);

        if (isEmailExist)
            return Result.Failure(UserErrors.DublicatedEmail);

        var allowedRoles = (await _roleService.GetAllAsync(cancellationToken: cancellationToken)).Select(x => x.Name);

        if (request.Roles.Except(allowedRoles).Any())
            return Result.Failure(RoleErrors.InvaildRoles);

        // Update
        user = request.Adapt(user);
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        await _context.UserRoles.Where(x => x.UserId == id).ExecuteDeleteAsync(cancellationToken);
        await _userManager.AddToRolesAsync(user, request.Roles);

        return Result.Success();

    }
    public async Task<Result> ToggleStatusAsync(string id, CancellationToken cancellationToken = default)
    {
        var affected = await _context.Users
             .Where(x => x.Id == id)
             .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.IsDisabled, x => !x.IsDisabled), cancellationToken);

        if (affected == 0)
            return Result.Failure(Errors<ApplicationUser>.NotFound);

        return Result.Success();
    }
    public async Task<Result> UnlockAsync(string id, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(Errors<ApplicationUser>.NotFound);

        if (user.LockoutEnd is null || user.LockoutEnd <= DateTimeOffset.UtcNow)
            return Result.Failure(UserErrors.UserAlreadyUnlocked);

        var result = await _userManager.SetLockoutEndDateAsync(user, null);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        return Result.Success();
    }

    // Profile
    public async Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _userManager.Users.Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .AsNoTracking()
            .SingleAsync(cancellationToken);

        return Result.Success(response);
    }
    public async Task UpdateProfileInfoAsync(string userId, UpdateProfileRequest request)
    {
        //var user = await _userManager.FindByIdAsync(userId);

        //user = request.Adapt(user);

        //await _userManager.UpdateAsync(user!);

        await _userManager.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setters =>
                 setters
                   .SetProperty(user => user.FirstName, request.FirstName)
                   .SetProperty(user => user.LastName, request.LastName)
            );
    }
    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    // private
    private async Task SendConfirmationEmailAsync(ApplicationUser user, string firstRole, string code)
    {
        // Email Sender
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        string emailBody = await EmailBodyBuilder.BuildEmailBodyAsync("ConfirmationEmailAndSetPassword", new Dictionary<string, string>
        {
            {"{{name}}",user.FirstName},
            {"{{role}}", firstRole},
            {"{{action_url}}",$"{origin}api/users/confirm-email-and-set-password?userId={user.Id}&code={code}"} 
            // frontEnd Diriction will open box to enter password
        });

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Join us at survey basket", emailBody));
    }
}
