
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Authentication;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            bool Authentic = await _userManager.CheckPasswordAsync(user, password);

            if (Authentic)
            {
                var (token, ExpiresIn) = await _jwtProvider.GenerateTokenAsync(user);
                return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, ExpiresIn);
            }
        }
        return null;
    }
}
