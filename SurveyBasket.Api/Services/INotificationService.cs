namespace SurveyBasket.Api.Services;

public interface INotificationService
{
    Task SendNewPollNotificationAsync(int? pollId = null);
}
