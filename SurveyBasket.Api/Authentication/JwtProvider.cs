// Ignore Spelling: Jwt

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SurveyBasket.Api.Authentication;

public class JwtProvider(IOptions<JwtOptions> jwtOption, UserManager<ApplicationUser> userManager) : IJwtProvider
{
    private readonly JwtOptions _jwtOption = jwtOption.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<(string token, int ExpiresIn)> GenerateTokenAsync(ApplicationUser user)
    {
        // Claims
        List<Claim> claims =
        [
               new Claim(ClaimTypes.Email,user.Email!),
               new Claim(ClaimTypes.NameIdentifier,user.Id),
               new Claim(ClaimTypes.Name,user.UserName!),
               new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        ];
        var userRoles = await _userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

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

        return (token: token, ExpiresIn: _jwtOption.ExpiryMinutes * 60);
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
