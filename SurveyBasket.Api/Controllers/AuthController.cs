
using OneOf;
using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return authResult.IsSuccess ? 
            Ok(authResult.Value) 
            :
            Problem(statusCode: StatusCodes.Status400BadRequest, title: authResult.Error.Code, detail: authResult.Error.Description);

    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var OneOFResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return OneOFResult.Match(
            response=>Ok(response),
            error=> Problem(statusCode: StatusCodes.Status400BadRequest, title: error.Code, detail: error.Description)
             );
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> revokeRefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var OneOFResult = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return OneOFResult.Match(
            Ok,
            error => Problem(statusCode: StatusCodes.Status400BadRequest, title: error.Code, detail: error.Description)
             );
    }
}
