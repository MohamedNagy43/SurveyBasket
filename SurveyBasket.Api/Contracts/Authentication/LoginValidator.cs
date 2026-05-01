// Ignore Spelling: Validator

using FluentValidation;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Authentication;

public class LoginValidator:AbstractValidator<LoginRequest>
{

    public LoginValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(r => r.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password require at least 8 character with at least 1 upper case , 1 lower case and 1 special character");
    }
}
