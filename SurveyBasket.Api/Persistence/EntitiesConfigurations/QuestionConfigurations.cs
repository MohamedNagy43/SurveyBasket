using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations;

public class QuestionConfigurations : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(e => new { e.Content, e.PollId }).IsUnique();
        builder.Property(e => e.Content).HasMaxLength(1000);
    }
}
