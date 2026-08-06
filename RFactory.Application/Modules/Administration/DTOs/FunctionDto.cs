namespace RFactory.Application.Modules.Administration.DTOs;

/// <summary>
/// Read model returned to API clients for a function (permission).
/// </summary>
public class FunctionDto
{
    public ulong Id { get; set; }
    public string FunctionCode { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public int? FunctionGroupId { get; set; }
}

/// <summary>
/// Payload for creating a function.
/// </summary>
public class CreateFunctionRequest
{
    public string FunctionCode { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public int? FunctionGroupId { get; set; }
}

/// <summary>
/// Payload for updating a function.
/// </summary>
public class UpdateFunctionRequest
{
    public string FunctionCode { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public int? FunctionGroupId { get; set; }
}

/// <summary>
/// What a catalogue sync wrote. <see cref="CatalogSize"/> is the total the application
/// enforces, so a run that added nothing still tells the admin the database is complete.
/// </summary>
public sealed record PermissionSyncResult(int GroupsCreated, int PermissionsCreated, int CatalogSize);
