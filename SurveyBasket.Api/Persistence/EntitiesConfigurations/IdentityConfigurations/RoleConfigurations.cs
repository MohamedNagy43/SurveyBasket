using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations.IdentityConfigurations;

public class RoleConfigurations : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        // seed data
        builder.HasData([
            new ApplicationRole{
                Id = DefaultRoles.AdminRoleId,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper(),
                ConcurrencyStamp = DefaultRoles.AdminConcurrencyStamp
            },
            new ApplicationRole{
                Id = DefaultRoles.MemberRoleId,
                Name = DefaultRoles.Member,
                NormalizedName = DefaultRoles.Member.ToUpper(),
                ConcurrencyStamp = DefaultRoles.MemberConcurrencyStamp,
                IsDefault = true
            },
        ]);
    }
}
