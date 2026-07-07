using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations.IdentityConfigurations;

public class RoleClaimsConfigurations : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        // seed permissions
        List<string> allPermissions = Permissions.GetAllPermissions();

        int counter = 1;
        var adminClaims = allPermissions.Select(x => new IdentityRoleClaim<string>
        {
            Id = counter++,
            RoleId = DefaultRoles.AdminRoleId,
            ClaimType = Permissions.Type,
            ClaimValue = x
        });

        builder.HasData(adminClaims);
    }
}
