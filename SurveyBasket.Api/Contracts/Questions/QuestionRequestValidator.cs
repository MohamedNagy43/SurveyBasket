// Ignore Spelling: Validator

using FluentValidation;

namespace SurveyBasket.Api.Contracts.Questions;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{

    public QuestionRequestValidator()
    {
        RuleFor(r => r.Content)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(r => r.Answers)
            .NotNull();

        RuleFor(r => r.Answers)
            .Must(answers => answers.Count >= 2)
            .When(r => r.Answers is not null)
            .WithMessage("Number of answers Must be two or more");

        RuleFor(r => r.Answers)
            .Must(answers => answers.Count() == answers.Distinct().Count())
            .When(r => r.Answers is not null)
            .WithMessage("answers Must be Unique to Specific Question");
    }

}
