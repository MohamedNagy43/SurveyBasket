namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[HasPermission(Permissions.Results)]
public class ResultsController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _resultService = resultService;

    [HttpGet("raw-data")]
    public async Task<IActionResult> PollVotes(int pollId,CancellationToken cancellationToken)
    {
        var result = await _resultService.GetPollVotesAsync(pollId,cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("votes-per-day")]
    public async Task<IActionResult> VotesPerDay(int pollId,CancellationToken cancellationToken)
    {
        var result = await _resultService.GetVotesPerDayAsync(pollId,cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("votes-per-question")]
    public async Task<IActionResult> VotesPerQuestion(int pollId,CancellationToken cancellationToken)
    {
        var result = await _resultService.GetVotesPerQuestionsAsync(pollId,cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
