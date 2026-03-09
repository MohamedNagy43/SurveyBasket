namespace SurveyBasket.Api.Errors;

public static class QuestionErrors
{
    public static Error DuplicatedContent => new Error("Question.DuplicatedContent"
    , "There is a Question with this content in this poll already", StatusCodes.Status409Conflict);
}
