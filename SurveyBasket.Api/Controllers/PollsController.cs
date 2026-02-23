
using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;
    

    [HttpGet,Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);
        return Ok(polls.Adapt<IEnumerable<PollResponse>>());
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        Poll? poll = await _pollService.GetAsync(id, cancellationToken);

        return poll is null ? NotFound() : Ok(poll.Adapt<PollResponse>());
    }


    [HttpPost]
    public async Task<IActionResult> Add(PollRequest request, CancellationToken cancellationToken)
    {
        var poll = await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = poll.Id }, poll);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PollRequest request, CancellationToken cancellationToken)
    {
        return await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken) ? NoContent() : NotFound();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await _pollService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }


    [HttpPut("{id}/TogglePublish")]
    public async Task<IActionResult> TogglePublish(int id, CancellationToken cancellationToken)
    {
        return await _pollService.TogglePublishStatusAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
