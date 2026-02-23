using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations;

public class UserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);


        //builder.Property(u=>u.RefreshTokens).HasMaxLength(5);
        

        // owned is "owned nav builder"
        builder.OwnsMany(u => u.RefreshTokens, owned =>
        {
            owned.ToTable("RefreshTokens");
            owned.WithOwner().HasForeignKey("UserId");
            owned.Property<int>("Id"); // shadow Property
            owned.HasKey("Id", "UserId");
        });

        // Elhelay
        //builder.OwnsMany(u=>u.RefreshTokens)
        //    .ToTable("RefreshTokens")
        //    .WithOwner()
        //    .HasForeignKey("UserId");
    }
}
