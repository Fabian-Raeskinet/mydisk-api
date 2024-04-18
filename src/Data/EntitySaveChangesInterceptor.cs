using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyDisks.Domain;

namespace MyDisks.Data;

public class EntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IDateTime _dateTime;
    private readonly IDomainEventDispatcher _dispatcher;

    public EntitySaveChangesInterceptor(IDateTime dateTime, IDomainEventDispatcher dispatcher)
    {
        _dateTime = dateTime;
        _dispatcher = dispatcher;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        // UpdateEntities(eventData.Context);
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        // UpdateEntities(eventData.Context);
        await DispatchDomainEvents(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    public async Task DispatchDomainEvents(DbContext? context)
    {
        if (context == null) return;

        var entities = context.ChangeTracker
            .Entries<Entity<Guid>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _dispatcher.Dispatch(domainEvent, CancellationToken.None);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        // foreach (var entry in context.ChangeTracker.Entries<Entity>())
        // {
        //     if (entry.State == EntityState.Added)
        //     {
        //         entry.Entity.CreatedAt = DateTimeOffset.Now;
        //         entry.Entity.UpdatedAt = DateTimeOffset.Now;
        //     }
        //
        //     if (entry.State is EntityState.Added or EntityState.Modified) entry.Entity.UpdatedAt = DateTimeOffset.Now;
        // }
    }
}