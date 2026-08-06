using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RFactory.Infrastructure.Data;

namespace RFactory.Infrastructure.Persistence;

/// <summary>
/// Default EF Core implementation of <see cref="IRepository{T}"/>. Reads are no-tracking;
/// deletes rely on the audit interceptor to convert them to soft-deletes for ISoftDelete entities.
/// </summary>
public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly RFactoryContext Context;
    protected readonly DbSet<T> Set;

    public GenericRepository(RFactoryContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Set = context.Set<T>();
    }

    public virtual async Task<List<T>> GetAll(CancellationToken ct = default)
        => await Set.AsNoTracking().ToListAsync(ct);

    public virtual async Task<T?> GetById(ulong id, CancellationToken ct = default)
        => await Set.FindAsync(new object?[] { id }, ct);

    public virtual async Task<List<T>> Where(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await Set.AsNoTracking().Where(predicate).ToListAsync(ct);

    public virtual async Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await Set.AsNoTracking().FirstOrDefaultAsync(predicate, ct);

    public virtual async Task<T> Add(T entity, CancellationToken ct = default)
    {
        await Set.AddAsync(entity, ct);
        await Context.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task<T> Update(T entity, CancellationToken ct = default)
    {
        Set.Update(entity);
        await Context.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task Delete(T entity, CancellationToken ct = default)
    {
        Set.Remove(entity);
        await Context.SaveChangesAsync(ct);
    }

    public virtual async Task<bool> DeleteById(ulong id, CancellationToken ct = default)
    {
        var entity = await Set.FindAsync(new object?[] { id }, ct);
        if (entity is null) return false;
        Set.Remove(entity);
        await Context.SaveChangesAsync(ct);
        return true;
    }

    public virtual async Task AddRange(IEnumerable<T> entities, CancellationToken ct = default)
    {
        var batch = entities as ICollection<T> ?? entities.ToList();
        if (batch.Count == 0) return;

        await Set.AddRangeAsync(batch, ct);
        await Context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteRange(IEnumerable<T> entities, CancellationToken ct = default)
    {
        var batch = entities as ICollection<T> ?? entities.ToList();
        if (batch.Count == 0) return;

        // Reads are AsNoTracking, so callers hand back detached entities. RemoveRange
        // attaches them by key, which is all the audit interceptor needs to flip them
        // to IsDeleted.
        Set.RemoveRange(batch);
        await Context.SaveChangesAsync(ct);
    }
}
