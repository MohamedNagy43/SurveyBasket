using FluentValidation;

namespace SurveyBasket.Api.Contracts.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(200)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password require at least 8 character with at least 1 upper case , 1 lower case and 1 special character");

        RuleFor(x => x.Roles)
            .NotEmpty()
            .Must(x => x.Count == x.Distinct().Count()).When(x => x.Roles is not null)
            .WithMessage("Can not add dublicated role to the same user");

    }
}
