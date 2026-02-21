// Ignore Spelling: Validator

using FluentValidation;

namespace SurveyBasket.Api.Contracts.Authentication;

public class LoginValidator:AbstractValidator<LoginRequest>
{

    public LoginValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(r => r.Password)
            .NotEmpty();
    }
}
