namespace SurveyBasket.Api.Errors;

public static class RoleErrors
{
    public static Error DuplicatedRole => new Error("Role.DuplicatedRole"
    , "There is a role with the same Name", StatusCodes.Status409Conflict);

    public static Error InvaildPermissions => new Error("Role.InvaildPermissions"
    , $"Permissions must be in the allowed permissions: [{string.Join(',', Permissions.GetAllPermissions())}]", StatusCodes.Status400BadRequest);
    public static Error InvaildRoles => new Error("User.InvaildRoles"
    , $"Roles must be in the allowed Roles", StatusCodes.Status400BadRequest);
}
