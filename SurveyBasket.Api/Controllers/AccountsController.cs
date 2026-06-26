using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Users;
using SurveyBasket.Api.Extension;

namespace SurveyBasket.Api.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class AccountsController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> GetProfileInfo(CancellationToken cancellationToken)
    {
        var result = await _userService.GetProfileInfoAsync(User.GetUserId()!, cancellationToken);
        return Ok(result.Value);
    }
    [HttpPut("info")]
    public async Task<IActionResult> UpdateProfileInfo(UpdateProfileRequest request)
    {
        await _userService.UpdateProfileInfoAsync(User.GetUserId()!, request);
        return NoContent();
    }
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
