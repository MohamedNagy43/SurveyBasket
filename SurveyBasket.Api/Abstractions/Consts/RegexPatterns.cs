namespace SurveyBasket.Api.Abstractions.Consts;

public static class RegexPatterns
{
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

    public const string SortDirction = "^(asc|ASC|desc|DESC)$";
}
