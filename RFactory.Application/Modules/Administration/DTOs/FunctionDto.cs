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
