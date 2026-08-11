namespace RFactory.Infrastructure.Persistence;

/// <summary>
/// Runs several repository calls as a single database transaction.
///
/// <see cref="IRepository{T}"/> saves on every call, which suits single-aggregate CRUD but
/// leaves a master-detail write half applied when a later call fails. Every repository in a
/// request resolves the same scoped <see cref="Data.RFactoryContext"/>, so wrapping the calls
/// here enlists all of their SaveChanges in one transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Runs <paramref name="work"/> inside a transaction and commits it. Any exception rolls
    /// the whole thing back. Nesting is safe: an inner call joins the transaction already in
    /// flight and leaves the commit to the outermost one.
    /// </summary>
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> work, CancellationToken ct = default);
}
