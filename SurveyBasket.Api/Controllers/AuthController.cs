
namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> Login(LoginRequest request,CancellationToken cancellationToken)
    {
        var authResponse = await _authService.GetTokenAsync(request.Email, request.Password,cancellationToken);

        return authResponse is null ? BadRequest("Invalid Email Or Password") : Ok(authResponse);
    }
}
