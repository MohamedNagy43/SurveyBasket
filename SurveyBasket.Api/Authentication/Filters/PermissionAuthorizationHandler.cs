namespace SurveyBasket.Api.Authentication.Filters;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequriement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequriement requirement)
    {
        var user = context.User.Identity;

        //if (user is null || !user.IsAuthenticated)
        //    return;

        //bool hasPermission = context.User.Claims.Any(x => x.Type == Permissions.Type && x.Value == requirement.Permission);

        //if (hasPermission)
        //    context.Succeed(requirement);

        //return;

        if (user is not { IsAuthenticated: true } || !context.User.Claims.Any(x => x.Type == Permissions.Type && x.Value == requirement.Permission))
            return;

        context.Succeed(requirement);
        return;

    }
}
