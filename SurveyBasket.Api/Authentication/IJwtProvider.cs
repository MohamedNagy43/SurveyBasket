// Ignore Spelling: Jwt

namespace SurveyBasket.Api.Authentication;

public interface IJwtProvider
{
    Task<(string token, int ExpiresIn)> GenerateTokenAsync(ApplicationUser user);
}
