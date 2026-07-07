using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations.IdentityConfigurations;

public class UserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);

        // Default data
        //var hasher = new PasswordHasher<ApplicationUser>();
        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.AdminId,
            FirstName = "SurveyBasket",
            LastName = "Admin",
            Email = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            UserName = DefaultUsers.AdminEmail.Split('@')[0],
            NormalizedUserName = DefaultUsers.AdminEmail.Split('@')[0].ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            PasswordHash = DefaultUsers.AdminPasswordHashed
        });

        // owned is "owned nav builder"
        builder.OwnsMany(u => u.RefreshTokens, owned =>
        {
            owned.ToTable("RefreshTokens");
            owned.WithOwner().HasForeignKey("UserId");
            owned.Property<int>("Id"); // shadow Property
            owned.HasKey("Id", "UserId");
        });
    }
}
