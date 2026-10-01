#nullable disable
using System;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Skill catalogue entry — a capability an employee can hold.
/// </summary>
public partial class Skill
{
    public ulong Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public string SkillCode { get; set; }

    public string SkillName { get; set; }

    /// <summary>Optional grouping label, e.g. "Safety", "Technical", "Language".</summary>
    public string SkillCategory { get; set; }

    public string Description { get; set; }
}
