namespace SurveyBasket.Api.Contracts.Results;

public record VoteResponse(
    string Voter,
    DateTime VoteDate,
    IEnumerable<QuestionAnswerResponse> SelectedAnswers
);
