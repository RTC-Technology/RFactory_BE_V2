namespace RFactory.Application.Modules.MasterData.DTOs;

/// <summary>
/// Read model returned to API clients for an area.
/// </summary>
public class AreaDto
{
    public ulong Id { get; set; }
    public long? FactoryId { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
}

/// <summary>
/// Payload for creating an area.
/// </summary>
public class CreateAreaRequest
{
    public long? FactoryId { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
}

/// <summary>
/// Payload for updating an area.
/// </summary>
public class UpdateAreaRequest
{
    public long? FactoryId { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
}
