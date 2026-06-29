using System.Text.Json;

namespace SurveyBasket.Api.Errors;

public static class RoleErrors
{
    public static Error DuplicatedRole => new Error("Role.DuplicatedRole"
    , "There is a role with the same Name", StatusCodes.Status409Conflict);

    public static Error InvaildPermissions => new Error("Role.InvaildPermissions"
    , $"Permissions must be in allowed permissions: [{string.Join(',',Permissions.GetAllPermissions())}]", StatusCodes.Status400BadRequest);
}
