namespace RFactory.Application.Modules.MasterData.DTOs;

/// <summary>
/// Read model for a working shift. <see cref="CrossDay"/> is stored as a nullable bit(1)
/// (ulong?) but exposed as a plain bool, same treatment as <c>User.IsAdmin</c>.
///
/// Times serialise as "HH:mm:ss". A shift whose <see cref="CrossDay"/> is set ends on the
/// following day, which is why an EndTime earlier than StartTime is legitimate.
/// </summary>
public class ShiftDto
{
    public ulong Id { get; set; }
    public string ShiftCode { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    /// <summary>Paid working minutes, i.e. the span minus whatever breaks are unpaid.</summary>
    public decimal? WorkingMinute { get; set; }
    public bool IsActive { get; set; }
    public bool CrossDay { get; set; }
}

public class CreateShiftRequest
{
    public string ShiftCode { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public decimal? WorkingMinute { get; set; }
    public bool IsActive { get; set; } = true;
    public bool CrossDay { get; set; }
}

public class UpdateShiftRequest
{
    public string ShiftCode { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public decimal? WorkingMinute { get; set; }
    public bool IsActive { get; set; }
    public bool CrossDay { get; set; }
}

/// <summary>
/// Read model for a break inside a shift. <see cref="SortOrder"/> sequences the breaks
/// within their own shift, not across the whole table.
/// </summary>
public class ShiftBreakDto
{
    public ulong Id { get; set; }
    public long? ShiftId { get; set; }
    public string BreakName { get; set; } = string.Empty;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? SortOrder { get; set; }
}

public class CreateShiftBreakRequest
{
    public long? ShiftId { get; set; }
    public string BreakName { get; set; } = string.Empty;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? SortOrder { get; set; }
}

public class UpdateShiftBreakRequest
{
    public long? ShiftId { get; set; }
    public string BreakName { get; set; } = string.Empty;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? SortOrder { get; set; }
}
