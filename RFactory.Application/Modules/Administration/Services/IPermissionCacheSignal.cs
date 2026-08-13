using Microsoft.Extensions.Primitives;

namespace RFactory.Application.Modules.Administration.Services;

/// <summary>
/// Evicts every cached permission set at once.
///
/// Deliberately not per user. Changing one group's functions changes what *every member of
/// that group* holds, so a per-user key would have to walk the membership to know whom to
/// evict — and missing one member leaves them holding a permission that was taken away.
/// Granting rights is a rare administrative action; a blunt flush is the right trade against
/// a silent authorization hole.
/// </summary>
public interface IPermissionCacheSignal
{
    /// <summary>Register on a cache entry so <see cref="Invalidate"/> drops it.</summary>
    IChangeToken Token { get; }

    /// <summary>Call after anything that changes who is granted what.</summary>
    void Invalidate();
}
