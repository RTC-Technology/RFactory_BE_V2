#nullable disable
using System;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Records that an employee holds a particular skill, at an optional proficiency level.
/// </summary>
public partial class EmployeeSkill
{
    public ulong Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public ulong EmployeeId { get; set; }

    public ulong SkillId { get; set; }

    /// <summary>1 = Beginner … 5 = Expert. Null means unrated.</summary>
    public int? ProficiencyLevel { get; set; }

    public DateTime? AcquiredDate { get; set; }

    public string Notes { get; set; }
}
