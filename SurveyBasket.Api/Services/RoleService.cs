using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool includeDisabled = false, CancellationToken cancellationToken = default)
        => await _roleManager.Roles
           .Where(x => !x.IsDefault && (!x.IsDeleted || includeDisabled))
           .AsNoTracking()
           .ProjectToType<RoleResponse>()
           .ToListAsync(cancellationToken);
    public async Task<Result<RoleDetailsResponse>> GetAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailsResponse>(Errors<ApplicationRole>.NotFound);

        var permissions = (await _roleManager.GetClaimsAsync(role))
            .Where(x => x.Type == Permissions.Type)
            .Select(x => x.Value);

        var response = new RoleDetailsResponse(id, role.Name!, role.IsDeleted, permissions);

        return Result.Success(response);
    }
    public async Task<Result<RoleDetailsResponse>> AddAsync(RoleRequest request, CancellationToken cancellationToken = default)
    {
        var isRoleExist = await _roleManager.RoleExistsAsync(request.Name);

        if (isRoleExist)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.DuplicatedRole);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailsResponse>(RoleErrors.InvaildPermissions);

        // adding role
        var newRole = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        var result = await _roleManager.CreateAsync(newRole);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<RoleDetailsResponse>(new Error(error.Code, error.Description, 400));
        }

        // adding permissions
        var permissions = request.Permissions
            .Select(x => new IdentityRoleClaim<string>()
            {
                RoleId = newRole.Id,
                ClaimType = Permissions.Type,
                ClaimValue = x
            });

        await _context.RoleClaims.AddRangeAsync(permissions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new RoleDetailsResponse(newRole.Id, newRole.Name, newRole.IsDeleted, request.Permissions));
    }
    public async Task<Result> UpdateAsync(string id, RoleRequest request, CancellationToken cancellationToken = default)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailsResponse>(Errors<ApplicationRole>.NotFound);

        bool NameExist = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id, cancellationToken);

        if (NameExist)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.DuplicatedRole);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailsResponse>(RoleErrors.InvaildPermissions);

        // update
        role.Name = request.Name;

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<RoleDetailsResponse>(new Error(error.Code, error.Description, 400));
        }

        var currentPermissions = await _context.RoleClaims
            .Where(x => x.RoleId == role.Id && x.ClaimType == Permissions.Type)
            .Select(x => x.ClaimValue).ToListAsync(cancellationToken);

        var newPermissions = request.Permissions.Except(currentPermissions)
             .Select(x => new IdentityRoleClaim<string>()
             {
                 RoleId = role.Id,
                 ClaimType = Permissions.Type,
                 ClaimValue = x
             });

        var remmovedPermissions = currentPermissions.Except(request.Permissions);

        // execute delete
        await _context.RoleClaims
            .Where(x => x.RoleId == role.Id && remmovedPermissions.Contains(x.ClaimValue))
            .ExecuteDeleteAsync(cancellationToken);

        await _context.RoleClaims.AddRangeAsync(newPermissions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> ToggleStatusAsync(string id, CancellationToken cancellationToken = default)
    {
        // diffrent code than Poll Toggle
        var isRoleExist = await _context.Roles.AnyAsync(x => x.Id == id, cancellationToken);

        if (!isRoleExist)
            return Result.Failure<RoleDetailsResponse>(Errors<ApplicationRole>.NotFound);

        await _roleManager.Roles
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.IsDeleted, x => !x.IsDeleted), cancellationToken);

        return Result.Success();
    }
}
