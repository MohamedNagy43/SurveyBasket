using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting(RateLimitingPolicies.IpLimit)]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("Register")]
    [DisableRateLimiting]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);

        return result.IsSuccess ? Ok("Email has been confirmed") : result.ToProblem();
    }
    [HttpPost("resend-email-confirmation-code")]
    public async Task<IActionResult> ResendEmailConfirmationCode(ResendEmailConfirmationCodeRequest request)
    {
        var result = await _authService.ResendEmailConfirmationCodeAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("forget-password")]
    public async Task<IActionResult> SendResetPasswordCode(ForgetPasswordRequest request)
    {
        var result = await _authService.SendResetPasswordCodeAsync(request.Email);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordCodeAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var OneOFResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return OneOFResult.Match(response => Ok(response), error => error.ToProblem());
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
