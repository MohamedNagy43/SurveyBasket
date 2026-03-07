using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations;

public class AnswerConfigurations : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasIndex(e => new { e.Content, e.QuestionId }).IsUnique();
        builder.Property(e => e.Content).HasMaxLength(1000);
    }
}
