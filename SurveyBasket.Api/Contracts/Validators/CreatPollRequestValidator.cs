using FluentValidation;
using SurveyBasket.Api.Contracts.Requests;

namespace SurveyBasket.Api.Contracts.Validators;

public class CreatPollRequestValidator:AbstractValidator<CreatePollRequest>
{
    public CreatPollRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(r => r.Description)
            .NotEmpty()
            .Length(3, 1000);
    }
}
