namespace RFactory.Application.Modules.HumanResources.DTOs;

public class EmployeeSkillDto
{
    public ulong Id { get; set; }
    public ulong EmployeeId { get; set; }
    public ulong SkillId { get; set; }

    /// <summary>Denormalised from the Skill table for display.</summary>
    public string? SkillCode { get; set; }
    public string? SkillName { get; set; }
    public string? SkillCategory { get; set; }

    /// <summary>1 = Beginner … 5 = Expert. Null means unrated.</summary>
    public int? ProficiencyLevel { get; set; }
    public DateTime? AcquiredDate { get; set; }
    public string? Notes { get; set; }
}

public class CreateEmployeeSkillRequest
{
    public ulong EmployeeId { get; set; }
    public ulong SkillId { get; set; }
    public int? ProficiencyLevel { get; set; }
    public DateTime? AcquiredDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateEmployeeSkillRequest
{
    public ulong SkillId { get; set; }
    public int? ProficiencyLevel { get; set; }
    public DateTime? AcquiredDate { get; set; }
    public string? Notes { get; set; }
}
