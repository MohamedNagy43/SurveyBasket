
using OneOf;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int _refreshTokenExpirationDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            bool Authentic = await _userManager.CheckPasswordAsync(user, password);

            if (Authentic)
            {
                var (accesstoken, expiresIn) = await _jwtProvider.GenerateTokenAsync(user);
                var newRefreshToken = new RefreshToken
                {
                    Token = generateRefreshToken(),
                    ExpiresOn = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
                };

                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);


                return Result.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, accesstoken, expiresIn, newRefreshToken.Token, newRefreshToken.ExpiresOn));
            }
        }
        return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
    }
    public async Task<OneOf<AuthResponse,Error>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        // Access Token Validation
        string? UserId = _jwtProvider.ValidateToken(token, validateLifeTime: false);
        if (UserId is null)
            return UserErrors.InvalidTokens;

        var user = await _userManager.FindByIdAsync(UserId);
        if (user is null)
            return UserErrors.InvalidTokens;


        // Refresh Token Validation
        RefreshToken? existToken = user.RefreshTokens.SingleOrDefault(refresh => refresh.Token == refreshToken && refresh.IsActive);

        if (existToken is null)
            return UserErrors.InvalidTokens;


        existToken.Revoke();

        // Generate New Access Token and Refresh Token

        var (accesstoken, expiresIn) = await _jwtProvider.GenerateTokenAsync(user);
        var newRefreshToken = new RefreshToken
        {
            Token = generateRefreshToken(),
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
    private string generateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
