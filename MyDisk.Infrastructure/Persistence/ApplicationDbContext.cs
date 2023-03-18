
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyDisk.Domain.Entities;
using MyDisk.Infrastructure.Interfaces;
using MyDisk.Infrastructure.Persistence.Interceptors;

namespace MyDisk.Infrastructure.Persistence;

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
    public DbSet<Disk> Disks => Set<Disk>();
    public DbSet<Author> Authors => Set<Author>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_saveChangesInterceptor != null) optionsBuilder.AddInterceptors(_saveChangesInterceptor);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
}