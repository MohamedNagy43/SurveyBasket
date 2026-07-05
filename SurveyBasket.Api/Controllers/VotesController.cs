using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Extension;

namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize(Roles = DefaultRoles.Member)]
[EnableRateLimiting(RateLimitingPolicies.ConcurrencyLimit)]
public class VotesController(IQuestionService questionService, IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;

    [HttpGet]
    public async Task<IActionResult> Start(int pollId, CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAvailableAsync(pollId, User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> Vote(int pollId, VoteRequest request, CancellationToken cancellationToken)
    {
        var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? Created() : result.ToProblem();
    }
}
