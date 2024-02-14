using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyDisks.Data.Configurations.Disks;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;

namespace MyDisks.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly EntitySaveChangesInterceptor? _saveChangesInterceptor;

    public ApplicationDbContext
    (
        DbContextOptions<ApplicationDbContext> options,
        EntitySaveChangesInterceptor? saveChangesInterceptor
    ) : base(options)
    {
        _saveChangesInterceptor = saveChangesInterceptor;
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Disk> Disks => Set<Disk>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Review> Reviews => Set<Review>();

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(DiskConfiguration))!);

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_saveChangesInterceptor != null) optionsBuilder.AddInterceptors(_saveChangesInterceptor);
    }
}