using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRM.Shared.Enums;

namespace HRM.Shared.Models;

public class Evaluation : BaseEntity
{
    public int EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }

    [Required, MaxLength(200)]
    public string Period { get; set; } = string.Empty;
    public decimal FinalScore { get; set; }
    public int Rating { get; set; }
    [MaxLength(200)]
    public string? Evaluator { get; set; }
    public EvaluationStatus Status { get; set; } = EvaluationStatus.Draft;
    [MaxLength(2000)]
    public string? Comments { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class Appraisal : BaseEntity
{
    public int EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }

    [Required, MaxLength(200)]
    public string Period { get; set; } = string.Empty;
    [MaxLength(2000)]
    public string? Goals { get; set; }
    [MaxLength(2000)]
    public string? Achievements { get; set; }
    [MaxLength(2000)]
    public string? ManagerComments { get; set; }
    public decimal OverallRating { get; set; }
    public AppraisalStatus Status { get; set; } = AppraisalStatus.Draft;
}

public class Promotion : BaseEntity
{
    public int EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }

    public int FromDesignationId { get; set; }
    [ForeignKey(nameof(FromDesignationId))]
    public Designation? FromDesignation { get; set; }

    public int ToDesignationId { get; set; }
    [ForeignKey(nameof(ToDesignationId))]
    public Designation? ToDesignation { get; set; }

    public DateTime EffectiveDate { get; set; }
    [MaxLength(500)]
    public string? Reason { get; set; }
    public PromotionStatus Status { get; set; } = PromotionStatus.Pending;
    public string? ApprovedBy { get; set; }
}

public class Transfer : BaseEntity
{
    public int EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }

    public int FromBranchId { get; set; }
    [ForeignKey(nameof(FromBranchId))]
    public Branch? FromBranch { get; set; }

    public int ToBranchId { get; set; }
    [ForeignKey(nameof(ToBranchId))]
    public Branch? ToBranch { get; set; }

    public int FromDepartmentId { get; set; }
    [ForeignKey(nameof(FromDepartmentId))]
    public Department? FromDepartment { get; set; }

    public int ToDepartmentId { get; set; }
    [ForeignKey(nameof(ToDepartmentId))]
    public Department? ToDepartment { get; set; }

    public DateTime EffectiveDate { get; set; }
    [MaxLength(500)]
    public string? Reason { get; set; }
    public TransferStatus Status { get; set; } = TransferStatus.Pending;
    [MaxLength(100)]
    public string? PipelineStage { get; set; }
}
