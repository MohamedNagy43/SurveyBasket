using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace SurveyBasket.Api.Authentication;

public class JwtProvider(IOptions<JwtOptions> jwtOption) : IJwtProvider
{
    private readonly JwtOptions _jwtOption = jwtOption.Value;
    public async Task<(string token, int ExpiresIn)> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        // Claims
        List<Claim> claims =
        [
               new Claim(ClaimTypes.Email,user.Email!),
               new Claim(ClaimTypes.NameIdentifier,user.Id),
               new Claim(ClaimTypes.Name,user.UserName!),
               new Claim(JwtRegisteredClaimNames.Jti,Guid.CreateVersion7().ToString()),
               new Claim(nameof(roles),JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
               new Claim(nameof(permissions),JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray)
        ];

        // signingCredentials
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var jwtSecurityToken = new JwtSecurityToken
        (
            issuer: _jwtOption.Issuer,
            audience: _jwtOption.Audience,
            expires: DateTime.UtcNow.AddMinutes(_jwtOption.ExpiryMinutes),
            claims: claims,
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return (token, ExpiresIn: _jwtOption.ExpiryMinutes * 60);
    }

    public string? ValidateToken(string token, bool ValidateLifeTime = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = ValidateLifeTime,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var SecurityToken = (JwtSecurityToken)validatedToken;

            return SecurityToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
        catch
        {
            return null;
        }
    }
}
