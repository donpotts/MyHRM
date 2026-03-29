using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRM.Shared.Enums;

namespace HRM.Shared.Models;

public class SalaryGrade : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public decimal BasicSalary { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Employee> Employees { get; set; } = [];
    public ICollection<SalaryGradeIncome> Incomes { get; set; } = [];
    public ICollection<SalaryGradeDeduction> Deductions { get; set; } = [];
}

public class IncomeComponent : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class DeductionComponent : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SalaryGradeIncome : BaseEntity
{
    public int SalaryGradeId { get; set; }
    [ForeignKey(nameof(SalaryGradeId))]
    public SalaryGrade? SalaryGrade { get; set; }

    public int IncomeComponentId { get; set; }
    [ForeignKey(nameof(IncomeComponentId))]
    public IncomeComponent? IncomeComponent { get; set; }

    public decimal Amount { get; set; }
}

public class SalaryGradeDeduction : BaseEntity
{
    public int SalaryGradeId { get; set; }
    [ForeignKey(nameof(SalaryGradeId))]
    public SalaryGrade? SalaryGrade { get; set; }

    public int DeductionComponentId { get; set; }
    [ForeignKey(nameof(DeductionComponentId))]
    public DeductionComponent? DeductionComponent { get; set; }

    public decimal Amount { get; set; }
}

public class EmployeePayroll : BaseEntity
{
    public int EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }
    public string Period => $"{new DateTime(Year, Month, 1):MMMM yyyy}";
    public decimal BasicSalary { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalDeduction { get; set; }
    public decimal TakeHomePay => BasicSalary + TotalIncome - TotalDeduction;
    public PayrollStatus Status { get; set; } = PayrollStatus.Draft;
    public ICollection<PayrollIncomeItem> IncomeItems { get; set; } = [];
    public ICollection<PayrollDeductionItem> DeductionItems { get; set; } = [];
}

public class PayrollIncomeItem : BaseEntity
{
    public int EmployeePayrollId { get; set; }
    [ForeignKey(nameof(EmployeePayrollId))]
    public EmployeePayroll? EmployeePayroll { get; set; }

    public int IncomeComponentId { get; set; }
    [ForeignKey(nameof(IncomeComponentId))]
    public IncomeComponent? IncomeComponent { get; set; }

    public decimal Amount { get; set; }
}

public class PayrollDeductionItem : BaseEntity
{
    public int EmployeePayrollId { get; set; }
    [ForeignKey(nameof(EmployeePayrollId))]
    public EmployeePayroll? EmployeePayroll { get; set; }

    public int DeductionComponentId { get; set; }
    [ForeignKey(nameof(DeductionComponentId))]
    public DeductionComponent? DeductionComponent { get; set; }

    public decimal Amount { get; set; }
}
