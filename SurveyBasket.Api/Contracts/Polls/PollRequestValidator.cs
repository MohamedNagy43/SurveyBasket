// Ignore Spelling: Validator

using FluentValidation;

namespace SurveyBasket.Api.Contracts.Polls;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    private readonly IPollService _pollService;

    public PollRequestValidator(IPollService pollService)
    {
        _pollService = pollService;

        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage("Title Must Be Not Null Or Empty")
            .Length(3, 100);

        RuleFor(r => r.Summary)
            .NotEmpty()
            .Length(3, 1500);

        RuleFor(r => r.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("StartDate Should not be older");

        RuleFor(r => r.EndsAt)
            .NotEmpty();

        RuleFor(r => r)
            .Must(r => r.EndsAt >= r.StartsAt)
            .WithName(r => nameof(r.EndsAt))
            .WithMessage("{PropertyName} must be A Future Date that the star date");

    }
}
