using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CulinaryBlog.Domain.Common;


namespace CulinaryBlog.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();
    public DbSet<RecipeImage> RecipeImages => Set<RecipeImage>();
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<uint>("RowVersion")
                    .HasColumnName("xmin")
                    .HasColumnType("xid")
                    .ValueGeneratedOnAddOrUpdate()
                    .IsRowVersion();
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        var changed = ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted).ToList();
        // All child mutations participate in the recipe's optimistic concurrency check.
        var recipeIds = changed.Select(e => e.Entity switch
        {
            RecipeStep x => x.RecipeId, RecipeIngredient x => x.RecipeId, RecipeImage x => x.RecipeId, _ => Guid.Empty
        }).Where(id => id != Guid.Empty).Distinct().ToArray();
        foreach (var recipe in ChangeTracker.Entries<Recipe>().Where(e => recipeIds.Contains(e.Entity.Id) && e.State == EntityState.Unchanged))
            recipe.Entity.UpdatedAt = DateTime.UtcNow;
        foreach (var entry in changed)
        {
            if (entry.Entity is BaseEntity entity)
            {
                if (entry.State == EntityState.Added) entity.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified) entity.UpdatedAt = DateTime.UtcNow;
                // Deletions remain physical; recipe children cascade as FR-RCP-007 specifies.
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
