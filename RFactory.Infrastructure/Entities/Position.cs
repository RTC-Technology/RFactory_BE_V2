#nullable disable
using System;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Job position / title master data.
/// </summary>
public partial class Position
{
    public ulong Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public string PositionCode { get; set; }

    public string PositionName { get; set; }

    /// <summary>Department or functional group this position belongs to.</summary>
    public string Department { get; set; }

    public string Description { get; set; }

    public bool? IsActive { get; set; }
}
