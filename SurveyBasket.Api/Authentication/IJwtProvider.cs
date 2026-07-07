namespace SurveyBasket.Api.Authentication;

public interface IJwtProvider
{
    Task<(string token, int ExpiresIn)> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
    /// <summary>
    /// Return Userid of valid access token,return null if validation fails
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    string? ValidateToken(string token, bool validateLifeTime = true);
}