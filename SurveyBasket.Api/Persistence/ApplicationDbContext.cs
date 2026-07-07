

using SurveyBasket.Api.Extension;

namespace SurveyBasket.Api.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Change all cascade to Restrict
        var CascadeForignKeys = modelBuilder.Model.GetEntityTypes()
             .SelectMany(t => t.GetForeignKeys())
             .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach (var foreignKey in CascadeForignKeys)
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;


        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteAnswer> VoteAnswers { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddAuditInformaiton();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddAuditInformaiton()
    {
        var AuditableEntries = ChangeTracker
            .Entries<AuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        string? userId = _httpContextAccessor.HttpContext?.User.GetUserId();

        foreach (var entry in AuditableEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(e => e.CreatedById).CurrentValue = userId!;
                entry.Property(e => e.CreatedOn).CurrentValue = DateTime.UtcNow;

            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.CreatedOn).IsModified = false;
                entry.Property(e => e.CreatedById).IsModified = false;

                entry.Property(e => e.UpdatedOn).CurrentValue = DateTime.UtcNow;
                entry.Property(e => e.UpdatedById).CurrentValue = userId;
            }
        }
    }
}
