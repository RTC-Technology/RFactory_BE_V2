namespace RFactory.Application.Modules.MasterData.DTOs;

/// <summary>
/// Read model returned to API clients for an organization.
/// </summary>
public class OrganizationDto
{
    public ulong Id { get; set; }
    public string OrganizationCode { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public long? ParentId { get; set; }
}

/// <summary>
/// Payload for creating an organization.
/// </summary>
public class CreateOrganizationRequest
{
    public string OrganizationCode { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public long? ParentId { get; set; }
}

/// <summary>
/// Payload for updating an organization.
/// </summary>
public class UpdateOrganizationRequest
{
    public string OrganizationCode { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public long? ParentId { get; set; }
}
