namespace RFactory.Application.Modules.Equipment.DTOs;

/// <summary>
/// Read model returned to API clients for a machine type.
/// </summary>
public class MachineTypeDto
{
    public ulong Id { get; set; }
    public string MachineTypeCode { get; set; } = string.Empty;
    public string MachineTypeName { get; set; } = string.Empty;
}

/// <summary>
/// Payload for creating a machine type.
/// </summary>
public class CreateMachineTypeRequest
{
    public string MachineTypeCode { get; set; } = string.Empty;
    public string MachineTypeName { get; set; } = string.Empty;
}

/// <summary>
/// Payload for updating a machine type.
/// </summary>
public class UpdateMachineTypeRequest
{
    public string MachineTypeCode { get; set; } = string.Empty;
    public string MachineTypeName { get; set; } = string.Empty;
}
