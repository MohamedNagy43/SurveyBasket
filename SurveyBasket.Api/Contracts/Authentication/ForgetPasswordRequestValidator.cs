using FluentValidation;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contracts.Authentication;

public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{

    public ForgetPasswordRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
