using FluentValidation;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Users;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{

    public ChangePasswordRequestValidator()
    {
        RuleFor(r => r.CurrentPassword)
            .NotEmpty();

        RuleFor(r => r.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password require at least 8 character with at least 1 upper case , 1 lower case and 1 special character")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New passwor cannot be the same as current password");
    }
}
