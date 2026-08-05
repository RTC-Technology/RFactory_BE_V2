namespace RFactory.Application.Modules.Equipment.DTOs;

/// <summary>
/// Read model returned to API clients for a machine.
/// </summary>
public class MachineDto
{
    public ulong Id { get; set; }
    public long? LineId { get; set; }
    public long? MachineTypeId { get; set; }
    public string MachineCode { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public int? Status { get; set; }
}

/// <summary>
/// Payload for creating a machine.
/// </summary>
public class CreateMachineRequest
{
    public long? LineId { get; set; }
    public long? MachineTypeId { get; set; }
    public string MachineCode { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public int? Status { get; set; }
}

/// <summary>
/// Payload for updating a machine.
/// </summary>
public class UpdateMachineRequest
{
    public long? LineId { get; set; }
    public long? MachineTypeId { get; set; }
    public string MachineCode { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public int? Status { get; set; }
}
