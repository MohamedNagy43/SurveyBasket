

using SurveyBasket.Api.Persistence.Migrations;

namespace SurveyBasket.Api.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
    : IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    public DbSet<Poll> Polls { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        addAuditInformaiton();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void addAuditInformaiton()
    {
        var AuditableEntries = ChangeTracker.Entries<AuditableEntity>();

        string? userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        foreach (var entry in AuditableEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(e => e.CreatedById).CurrentValue = userId!;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.UpdatedOn).CurrentValue = DateTime.UtcNow;
                entry.Property(e => e.UpdatedById).CurrentValue = userId;
            }
        }
    }
}
