
namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var authResponse = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return authResponse is null ? BadRequest("Invalid Email Or Password") : Ok(authResponse);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResponse = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResponse is null ? BadRequest("Invalid Tokens") : Ok(authResponse);
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> revokeRefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var IsRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return IsRevoked ? NoContent() : BadRequest("Operation Failed");
    }
}
