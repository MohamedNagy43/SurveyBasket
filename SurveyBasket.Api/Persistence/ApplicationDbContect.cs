
using System.Reflection;

namespace SurveyBasket.Api.Persistence;

public class ApplicationDbContect(DbContextOptions<ApplicationDbContect> options):DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Poll> Polls { get; set; }
}
