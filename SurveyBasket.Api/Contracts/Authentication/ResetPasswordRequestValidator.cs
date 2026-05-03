using FluentValidation;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Authentication;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password require at least 8 character with at least 1 upper case , 1 lower case and 1 special character");

        RuleFor(x => x.Code)
            .NotEmpty();

    }
}
