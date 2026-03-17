namespace SurveyBasket.Api.Errors;

public static class VoteErrors
{
    public static Error DuplicatedVote => new("Vote.DuplicatedVote"
    , "User has already voted on this poll before", StatusCodes.Status409Conflict);

    public static Error InvalidQuestions => new("Vote.InvalidQuestions"
    , "InvalidQuestions", StatusCodes.Status400BadRequest);
    public static Error InvalidAnswers => new("Vote.InvalidAnswers"
    , "InvalidAnswers", StatusCodes.Status400BadRequest);
}
