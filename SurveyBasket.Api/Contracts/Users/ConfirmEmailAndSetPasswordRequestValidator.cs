using FluentValidation;

namespace SurveyBasket.Api.Contracts.Users;

public class ConfirmEmailAndSetPasswordRequestValidator : AbstractValidator<ConfirmEmailAndSetPasswordRequest>
{
    public ConfirmEmailAndSetPasswordRequestValidator()
    {
        RuleFor(x => x.UserId)
           .NotEmpty();

        RuleFor(x => x.Code)
           .NotEmpty();

        RuleFor(r => r.Password)
           .NotEmpty()
           .Matches(RegexPatterns.Password)
           .WithMessage("Password require at least 8 character with at least 1 upper case , 1 lower case and 1 special character");

    }
}
