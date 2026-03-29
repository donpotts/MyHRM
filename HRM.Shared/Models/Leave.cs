using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRM.Shared.Enums;

namespace HRM.Shared.Models;

public class LeaveCategory : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public int MaxDaysPerYear { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = [];
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = [];
}

public class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }

    public int LeaveCategoryId { get; set; }
    [ForeignKey(nameof(LeaveCategoryId))]
    public LeaveCategory? LeaveCategory { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    [MaxLength(500)]
    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    [MaxLength(500)]
    public string? RejectionReason { get; set; }
}

public class LeaveBalance : BaseEntity
{
    public int EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }

    public int LeaveCategoryId { get; set; }
    [ForeignKey(nameof(LeaveCategoryId))]
    public LeaveCategory? LeaveCategory { get; set; }

    public int Year { get; set; }
    public int TotalEntitled { get; set; }
    public int Used { get; set; }
    public int Remaining => TotalEntitled - Used;
}
