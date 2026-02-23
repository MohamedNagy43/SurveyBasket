// Ignore Spelling: Validator

using FluentValidation;

namespace SurveyBasket.Api.Contracts.Authentication;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(r => r.Token)
            .NotEmpty();

        RuleFor(r => r.RefreshToken)
            .NotEmpty();
    }
}
