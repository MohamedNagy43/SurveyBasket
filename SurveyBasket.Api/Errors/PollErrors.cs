namespace SurveyBasket.Api.Errors;

public static class PollErrors
{
    public static Error DuplicatedTitle => new Error("Poll.DuplicatedTitle"
    , "There is a poll with the same title", StatusCodes.Status409Conflict);
}
