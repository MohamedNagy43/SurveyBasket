// Ignore Spelling: Jwt

using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SurveyBasket.Api.Authentication;

public class JwtProvider(IConfiguration configuration, UserManager<ApplicationUser> userManager) : IJwtProvider
{
    private readonly IConfiguration _configuration = configuration;
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
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var jwtSecurityToken = new JwtSecurityToken
        (
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
            claims: claims,
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return (token: token, ExpiresIn: int.Parse(_configuration["Jwt:ExpiresInMinutes"]!) * 60);
    }
}
