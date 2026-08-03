using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RFactory.Shared.Abstractions;

namespace RFactory.Infrastructure.Data.Interceptors;

/// <summary>
/// Populates audit columns (CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) and converts hard deletes 
/// into soft deletes (IsDeleted) for entities that have these properties. Uses reflection to detect 
/// and set properties, so it works with Database First scaffolded entities without requiring interfaces.
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IUser _user;
    private readonly TimeProvider _clock;

    public AuditableEntityInterceptor(IUser user, TimeProvider clock)
    {
        _user = user;
        _clock = clock;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null) return;

        var now = _clock.GetUtcNow().UtcDateTime;
        var userId = _user.Id;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            var entityType = entry.Entity.GetType();
            
            // Handle audit fields for Added or Modified entities
            if (entry.State == EntityState.Added)
            {
                SetPropertyValue(entry.Entity, entityType, "CreatedBy", userId);
                SetPropertyValue(entry.Entity, entityType, "CreatedDate", now);
                SetPropertyValue(entry.Entity, entityType, "UpdatedBy", userId);
                SetPropertyValue(entry.Entity, entityType, "UpdatedDate", now);
            }

            if (entry.State == EntityState.Modified)
            {
                SetPropertyValue(entry.Entity, entityType, "UpdatedBy", userId);
                SetPropertyValue(entry.Entity, entityType, "UpdatedDate", now);
            }

            // Handle soft delete: convert hard delete to soft delete
            if (entry.State == EntityState.Deleted)
            {
                var isDeletedProp = entityType.GetProperty("IsDeleted");
                if (isDeletedProp != null)
                {
                    entry.State = EntityState.Modified;
                    
                    // Handle both bool and bool? types
                    if (isDeletedProp.PropertyType == typeof(bool))
                    {
                        isDeletedProp.SetValue(entry.Entity, true);
                    }
                    else if (isDeletedProp.PropertyType == typeof(bool?))
                    {
                        isDeletedProp.SetValue(entry.Entity, (bool?)true);
                    }
                    
                    SetPropertyValue(entry.Entity, entityType, "UpdatedBy", userId);
                    SetPropertyValue(entry.Entity, entityType, "UpdatedDate", now);
                }
            }
        }
    }

    private static void SetPropertyValue(object entity, Type entityType, string propertyName, object? value)
    {
        var property = entityType.GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(entity, value);
        }
    }
}
