using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Contracts.Requests;
using SurveyBasket.Api.Contracts.Responses;
using SurveyBasket.Api.Contracts.Validators;
using SurveyBasket.Api.Services;
using System.ComponentModel.DataAnnotations;
using static SurveyBasket.Api.Controllers.PollsController;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_pollService.GetAll().Adapt<IEnumerable<PollResponse>>());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        Poll? poll = _pollService.Get(id);

        return poll is null ? NotFound() : Ok(poll.Adapt<PollResponse>());
    }

    [HttpPost]
    public IActionResult Add(CreatePollRequest request)
    {
        var poll = _pollService.Add(request.Adapt<Poll>());
        return CreatedAtAction(nameof(Get), new { id = poll.Id }, poll);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, CreatePollRequest request)
    {
        return _pollService.Update(id, request.Adapt<Poll>()) ? NoContent() : NotFound();
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return _pollService.Delete(id) ? NoContent() : NotFound();
    }
}
