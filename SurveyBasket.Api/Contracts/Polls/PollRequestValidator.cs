// Ignore Spelling: Validator

using FluentValidation;

namespace SurveyBasket.Api.Contracts.Polls;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    private readonly IPollService _pollService;

    public PollRequestValidator(IPollService pollService)
    {


        RuleFor(r => r.Title)
            .NotEmpty()
            .Must(beUniqueTitle).When(p=>!string.IsNullOrEmpty(p.Title))
            .WithMessage("Title must be Unique")
            .Length(3, 100);

        RuleFor(r => r.Summary)
            .NotEmpty()
            .Length(3, 1500);

        RuleFor(r => r.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(r => r.EndsAt)
            .NotEmpty();

        RuleFor(r => r)
            .Must(r => r.EndsAt >= r.StartsAt)
            .WithName(r=>nameof(r.EndsAt))
            .WithMessage("{PropertyName} must be A Future Date that the star date");
        _pollService = pollService;
    }
    private bool beUniqueTitle(string title)
    {
        return _pollService.GetAsync(p => p.Title == title) is null;
    }
}
