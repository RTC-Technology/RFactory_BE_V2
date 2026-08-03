namespace RFactory.Application.Modules.MasterData.DTOs;

/// <summary>
/// Read model returned to API clients for a factory.
/// </summary>
public class FactoryDto
{
    public ulong Id { get; set; }
    public string FactoryCode { get; set; } = string.Empty;
    public string FactoryName { get; set; } = string.Empty;
}

/// <summary>
/// Payload for creating a factory.
/// </summary>
public class CreateFactoryRequest
{
    public string FactoryCode { get; set; } = string.Empty;
    public string FactoryName { get; set; } = string.Empty;
}

/// <summary>
/// Payload for updating a factory.
/// </summary>
public class UpdateFactoryRequest
{
    public string FactoryCode { get; set; } = string.Empty;
    public string FactoryName { get; set; } = string.Empty;
}
