// Ignore Spelling: Validator

using FluentValidation;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Contracts.Polls;

public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
{
    public VoteAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0);

        RuleFor(x => x.AnswerId)
            .GreaterThan(0);

    }
}
