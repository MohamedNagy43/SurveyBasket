
namespace SurveyBasket.Api.Errors;

public record UserErrors
{
    public static Error InvalidCredentials => new Error("User.InvalidCredentials"
        , "Invalid Email Or Password", StatusCodes.Status401Unauthorized);
    public static Error UserDisabled => new Error("User.DisabledAccount"
        , "user account has been disabled,please contact the admin", StatusCodes.Status401Unauthorized);
    public static Error UserAlreadyUnlocked => new Error("User.UserAlreadyUnlocked"
        , "user account is unlocked and you can sign in", StatusCodes.Status401Unauthorized);

    public static Error InvalidEmailConfirmationCode => new Error("User.InvalidEmailConfirmationCode"
        , "Invalid Email Confirmation Code", StatusCodes.Status401Unauthorized);

    public static Error InvalidForgetPasswordCode => new Error("User.InvalidForgetPasswordCode"
        , "Invalid Forget PasswordCode Code", StatusCodes.Status401Unauthorized);

    public static Error EmailAlreadyConfirmed => new Error("User.EmailAlreadyConfirmed"
        , "Email Is Aleady confirmed for this user", StatusCodes.Status400BadRequest);

    public static Error InvalidTokens => new Error("User.InvalidTokens"
        , "Invalid Tokens", StatusCodes.Status400BadRequest);

    public static Error DublicatedEmail => new Error("User.DublicatedEmail"
        , "Email already has been taken", StatusCodes.Status409Conflict);

    public static Error UserLockedOut => new Error("User.LockedOut"
        , "You have enterd a wrong password so many times please try again later", StatusCodes.Status401Unauthorized);

    public static Error EmailNotConfirmed => new Error("User.EmailNotConfirmed"
        , "Email Has not been confirmed", StatusCodes.Status401Unauthorized);
}
