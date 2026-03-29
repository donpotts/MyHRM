using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.Shared.Models;

public class Branch : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Department> Departments { get; set; } = [];
    public ICollection<Employee> Employees { get; set; } = [];
}

public class Department : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public int BranchId { get; set; }
    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }
    public int? ParentDepartmentId { get; set; }
    [ForeignKey(nameof(ParentDepartmentId))]
    public Department? ParentDepartment { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Employee> Employees { get; set; } = [];
}

public class Designation : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Employee> Employees { get; set; } = [];
}

public class Employee : BaseEntity
{
    [Required, MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    [MaxLength(256)]
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public Enums.Gender? Gender { get; set; }
    public Enums.MaritalStatus? MaritalStatus { get; set; }
    public Enums.EmploymentStatus Status { get; set; } = Enums.EmploymentStatus.Active;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? AvatarUrl { get; set; }

    public int BranchId { get; set; }
    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }

    public int DepartmentId { get; set; }
    [ForeignKey(nameof(DepartmentId))]
    public Department? Department { get; set; }

    public int DesignationId { get; set; }
    [ForeignKey(nameof(DesignationId))]
    public Designation? Designation { get; set; }

    public int? SalaryGradeId { get; set; }
    [ForeignKey(nameof(SalaryGradeId))]
    public SalaryGrade? SalaryGrade { get; set; }

    public string? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    public decimal FTE { get; set; } = 1.00m;

    public ICollection<LeaveRequest> LeaveRequests { get; set; } = [];
    public ICollection<Evaluation> Evaluations { get; set; } = [];
    public ICollection<EmployeePayroll> Payrolls { get; set; } = [];
}
