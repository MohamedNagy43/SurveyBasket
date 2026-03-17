namespace SurveyBasket.Api.Extension;

public static class UserExtension
{
    public static string? GetUserId(this ClaimsPrincipal User)
        => User.FindFirstValue(ClaimTypes.NameIdentifier);
}
