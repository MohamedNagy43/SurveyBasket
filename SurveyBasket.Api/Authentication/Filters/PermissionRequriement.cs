namespace SurveyBasket.Api.Authentication.Filters;

public class PermissionRequriement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
