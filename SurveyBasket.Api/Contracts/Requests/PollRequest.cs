namespace SurveyBasket.Api.Contracts.Requests;

public record PollRequest(string Title, string Summary, DateOnly StartAt, DateOnly EndAt);
