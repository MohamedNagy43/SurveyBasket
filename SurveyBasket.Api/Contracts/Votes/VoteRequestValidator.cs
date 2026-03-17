// Ignore Spelling: Validator

using FluentValidation;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Contracts.Polls;

public class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(x => x.Answers)
            .NotEmpty();

        RuleForEach(x => x.Answers).SetInheritanceValidator(v=>v.Add(new VoteAnswerRequestValidator()));
    }
}
