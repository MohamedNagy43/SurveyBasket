namespace SurveyBasket.Api.Abstractions.Consts;

public static class RateLimitingPolicies
{
    public const string IpLimit = "ipLimit";
    public const string UserLimit = "userLimit";
    public const string ConcurrencyLimit = "concurrencyLimit";
}
