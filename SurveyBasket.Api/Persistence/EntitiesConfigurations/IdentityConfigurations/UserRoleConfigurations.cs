using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations.IdentityConfigurations;

public class UserRoleConfigurations : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        // seed data
        builder.HasData(new IdentityUserRole<string>
        {
            UserId = DefaultUsers.AdminId,
            RoleId = DefaultRoles.AdminRoleId,
        });
    }
}
