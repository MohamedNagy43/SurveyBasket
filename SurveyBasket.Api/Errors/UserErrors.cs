
namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static Error InvalidCredentials => new Error("User.InvalidCredentials"
        , "Invalid Email Or Password", StatusCodes.Status401Unauthorized);

    public static Error InvalidTokens => new Error("User.InvalidTokens"
        , "Invalid Tokens", StatusCodes.Status400BadRequest);
}
