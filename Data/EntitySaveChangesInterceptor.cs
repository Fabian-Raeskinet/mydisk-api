using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyDisk.Domain;
using MyDisk.Domain.Entities;

namespace MyDisks.Data;

public class EntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IDateTime _dateTime;

    public EntitySaveChangesInterceptor(IDateTime dateTime)
    {
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.Now;
                entry.Entity.UpdatedAt = DateTimeOffset.Now;
            }

            if (entry.State is EntityState.Added or EntityState.Modified) entry.Entity.UpdatedAt = DateTimeOffset.Now;
        }
    }
}