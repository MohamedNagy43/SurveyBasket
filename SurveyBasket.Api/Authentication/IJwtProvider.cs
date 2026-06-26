// Ignore Spelling: Jwt

namespace SurveyBasket.Api.Authentication;

public interface IJwtProvider
{
    Task<(string token, int ExpiresIn)> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
    /// <summary>
    /// Return UserID Of Valid Access Token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    string? ValidateToken(string token, bool validateLifeTime = true);
}