using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiVersion(1, Deprecated = true), ApiVersion(2)]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;


    [HttpGet]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpGet("current")]
    [MapToApiVersion(1)]
    [Authorize(Roles = DefaultRoles.Member)]
    [EnableRateLimiting(RateLimitingPolicies.UserLimit)]
    public async Task<IActionResult> GetCurrentV1(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetCurrentAsyncV1(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("current")]
    [MapToApiVersion(2)]
    [Authorize(Roles = DefaultRoles.Member)]
    [EnableRateLimiting(RateLimitingPolicies.UserLimit)]
    public async Task<IActionResult> GetCurrentV2(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetCurrentAsyncV2(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpGet("{id}")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPost]
    [HasPermission(Permissions.AddPolls)]
    public async Task<IActionResult> Add(PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.AddAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update(int id, PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeletePolls)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


    [HttpPut("{id}/TogglePublish")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> TogglePublish(int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
