using FluentValidation;

namespace SurveyBasket.Api.Contracts.Authentication;

public class ResendEmailConfirmationCodeRequestValidator : AbstractValidator<ResendEmailConfirmationCodeRequest>
{
    public ResendEmailConfirmationCodeRequestValidator()
    {

        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
