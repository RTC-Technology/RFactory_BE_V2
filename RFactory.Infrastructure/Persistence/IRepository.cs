using System.Linq.Expressions;

namespace RFactory.Infrastructure.Persistence;

/// <summary>
/// Generic persistence contract for aggregate CRUD. Lives in Infrastructure (next to entities)
/// so the Application layer depends on Infrastructure in a single direction, avoiding a cycle.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<List<T>> GetAll(CancellationToken ct = default);
    Task<T?> GetById(ulong id, CancellationToken ct = default);
    Task<List<T>> Where(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T> Add(T entity, CancellationToken ct = default);
    Task<T> Update(T entity, CancellationToken ct = default);
    Task Delete(T entity, CancellationToken ct = default);
    Task<bool> DeleteById(ulong id, CancellationToken ct = default);
}
