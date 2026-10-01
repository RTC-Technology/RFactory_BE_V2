namespace RFactory.Application.Modules.HumanResources.DTOs;

public class SkillDto
{
    public ulong Id { get; set; }
    public string SkillCode { get; set; } = string.Empty;
    public string SkillName { get; set; } = string.Empty;
    public string? SkillCategory { get; set; }
    public string? Description { get; set; }
}

public class CreateSkillRequest
{
    public string SkillCode { get; set; } = string.Empty;
    public string SkillName { get; set; } = string.Empty;
    public string? SkillCategory { get; set; }
    public string? Description { get; set; }
}

public class UpdateSkillRequest
{
    public string SkillCode { get; set; } = string.Empty;
    public string SkillName { get; set; } = string.Empty;
    public string? SkillCategory { get; set; }
    public string? Description { get; set; }
}
