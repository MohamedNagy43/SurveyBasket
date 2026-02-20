using FluentValidation;
using SurveyBasket.Api.Contracts.Requests;

namespace SurveyBasket.Api.Contracts.Validators;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {


        RuleFor(r => r.Title)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(r => r.Summary)
            .NotEmpty()
            .Length(3, 1500);

        RuleFor(r => r.StartAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(r => r.EndAt)
            .NotEmpty();

        RuleFor(r => r)
            .Must(r => r.EndAt >= r.StartAt)
            .WithName(r=>nameof(r.EndAt))
            .WithMessage("{PropertyName} must be A Future Date that the star date");

    }
}
