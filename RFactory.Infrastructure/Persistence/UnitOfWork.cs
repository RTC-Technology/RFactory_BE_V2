using Microsoft.EntityFrameworkCore;
using RFactory.Infrastructure.Data;

namespace RFactory.Infrastructure.Persistence;

/// <inheritdoc cref="IUnitOfWork" />
public class UnitOfWork : IUnitOfWork
{
    private readonly RFactoryContext _context;

    public UnitOfWork(RFactoryContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> work, CancellationToken ct = default)
    {
        // Someone up the stack already owns a transaction; opening a second one here would
        // commit their work early.
        if (_context.Database.CurrentTransaction is not null)
        {
            return await work(ct);
        }

        // The strategy may replay `work` on a transient failure, so the transaction is opened
        // inside it — a handle created outside would be dead by the second attempt. With no
        // retry policy configured this simply invokes the operation once.
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async token =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(token);
            var result = await work(token);
            await transaction.CommitAsync(token);
            return result;
        }, ct);
    }
}
