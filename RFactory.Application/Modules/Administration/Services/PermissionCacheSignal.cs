using Microsoft.Extensions.Primitives;

namespace RFactory.Application.Modules.Administration.Services;

/// <inheritdoc cref="IPermissionCacheSignal"/>
public sealed class PermissionCacheSignal : IPermissionCacheSignal
{
    private CancellationTokenSource _source = new();

    public IChangeToken Token => new CancellationChangeToken(Volatile.Read(ref _source).Token);

    public void Invalidate()
    {
        // Swap first, then cancel: a caller reading Token in between gets the fresh source
        // rather than one about to fire, so an entry written during an invalidation is not
        // evicted the instant it lands.
        var previous = Interlocked.Exchange(ref _source, new CancellationTokenSource());
        previous.Cancel();

        // Not disposed on purpose. A request that read the old token a moment ago may still
        // be registering on it, and registering against a disposed source throws. These are
        // rare (one per grant change) and collectable, so leaving them to the GC is cheaper
        // than the race.
    }
}
