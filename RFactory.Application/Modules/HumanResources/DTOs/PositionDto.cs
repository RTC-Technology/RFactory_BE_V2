namespace RFactory.Application.Modules.HumanResources.DTOs;

public class PositionDto
{
    public ulong Id { get; set; }
    public string PositionCode { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class CreatePositionRequest
{
    public string PositionCode { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class UpdatePositionRequest
{
    public string PositionCode { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
