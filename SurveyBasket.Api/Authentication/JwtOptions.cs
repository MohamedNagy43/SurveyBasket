namespace SurveyBasket.Api.Authentication;

public class JwtOptions
{
    public static string SectionName { get; } = "Jwt";


    [Required]
    public string SecretKey { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required, Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; init; }
}
