using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Shared.Abstractions;
using RFactory.Shared.Api;

namespace RFactory.API.Authorization;

/// <summary>How to combine several codes on one endpoint.</summary>
public enum PermissionMode
{
    /// <summary>Any one of the codes is enough. The default.</summary>
    Any,

    /// <summary>Every code is required.</summary>
    All,
}

/// <summary>
/// Gates an action or controller behind one or more permission codes, resolved from the
/// caller's user groups.
///
/// <code>
/// [RequirePermission(PermissionCodes.MenuManage)]
/// public async Task&lt;ActionResult&gt; Create(...) { }
/// </code>
///
/// Written as an authorization filter rather than a policy: a policy would need a
/// parameterised policy provider to carry the code, and the denial body has to be the
/// same <see cref="ApiResponse{T}"/> envelope every other endpoint returns, which the
/// default handler does not produce.
///
/// Assumes <c>[Authorize]</c> has already run — it still answers 401 rather than 403 for
/// an anonymous caller, so a controller that forgets the attribute fails closed.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _codes;

    public PermissionMode Mode { get; init; } = PermissionMode.Any;

    public RequirePermissionAttribute(params string[] codes)
    {
        _codes = codes;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var services = context.HttpContext.RequestServices;
        var currentUser = services.GetRequiredService<IUser>();

        if (!ulong.TryParse(currentUser.Id, out var userId))
        {
            context.Result = Deny("Authentication is required.", HttpStatusCode.Unauthorized);
            return;
        }

        // Admins bypass every check — the same rule the client applies, kept here so the
        // API does not depend on the client honouring it.
        if (currentUser.IsAdmin || _codes.Length == 0)
        {
            return;
        }

        var permissionService = services.GetRequiredService<IUserPermissionService>();
        var granted = await permissionService.GetForUserAsync(userId, context.HttpContext.RequestAborted);

        // Codes are typed by hand on the permission screen, so matching ignores case —
        // the same tolerance UserPermissionService applies when de-duplicating them.
        var held = new HashSet<string>(granted.Codes, StringComparer.OrdinalIgnoreCase);
        var allowed = Mode == PermissionMode.All
            ? _codes.All(held.Contains)
            : _codes.Any(held.Contains);

        if (!allowed)
        {
            context.Result = Deny(
                $"This action requires the '{string.Join("', '", _codes)}' permission.",
                HttpStatusCode.Forbidden);
        }
    }

    private static ObjectResult Deny(string message, HttpStatusCode status) =>
        new(ApiResponseFactory.Fail(message, status)) { StatusCode = (int)status };
}
