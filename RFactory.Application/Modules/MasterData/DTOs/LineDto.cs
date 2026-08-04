namespace RFactory.Application.Modules.MasterData.DTOs;

/// <summary>
/// Read model returned to API clients for a line.
/// </summary>
public class LineDto
{
    public ulong Id { get; set; }
    public long? AreaId { get; set; }
    public string LineCode { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;
    public int? Status { get; set; }
    public string? LayoutImage { get; set; }
}

/// <summary>
/// Payload for creating a line.
/// </summary>
public class CreateLineRequest
{
    public long? AreaId { get; set; }
    public string LineCode { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;
    public int? Status { get; set; }
    public string? LayoutImage { get; set; }
}

/// <summary>
/// Payload for updating a line.
/// </summary>
public class UpdateLineRequest
{
    public long? AreaId { get; set; }
    public string LineCode { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;
    public int? Status { get; set; }
    public string? LayoutImage { get; set; }
}
