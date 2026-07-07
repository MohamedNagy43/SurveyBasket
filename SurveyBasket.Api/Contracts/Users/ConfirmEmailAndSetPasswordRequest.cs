namespace SurveyBasket.Api.Contracts.Users;

public record ConfirmEmailAndSetPasswordRequest(
    string UserId,    
    string Code,
    string Password
);