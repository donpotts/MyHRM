using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HRM.Shared.Models;

namespace HRM.Server.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveCategory> LeaveCategories => Set<LeaveCategory>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<Evaluation> Evaluations => Set<Evaluation>();
    public DbSet<Appraisal> Appraisals => Set<Appraisal>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<SalaryGrade> SalaryGrades => Set<SalaryGrade>();
    public DbSet<IncomeComponent> IncomeComponents => Set<IncomeComponent>();
    public DbSet<DeductionComponent> DeductionComponents => Set<DeductionComponent>();
    public DbSet<SalaryGradeIncome> SalaryGradeIncomes => Set<SalaryGradeIncome>();
    public DbSet<SalaryGradeDeduction> SalaryGradeDeductions => Set<SalaryGradeDeduction>();
    public DbSet<EmployeePayroll> EmployeePayrolls => Set<EmployeePayroll>();
    public DbSet<PayrollIncomeItem> PayrollIncomeItems => Set<PayrollIncomeItem>();
    public DbSet<PayrollDeductionItem> PayrollDeductionItems => Set<PayrollDeductionItem>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<AutoNumber> AutoNumbers => Set<AutoNumber>();
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>()
            .HasIndex(e => e.EmployeeCode)
            .IsUnique();

        builder.Entity<Employee>()
            .Property(e => e.FTE)
            .HasPrecision(5, 2);

        builder.Entity<SalaryGrade>()
            .Property(s => s.BasicSalary)
            .HasPrecision(18, 2);

        builder.Entity<EmployeePayroll>()
            .Property(p => p.BasicSalary).HasPrecision(18, 2);
        builder.Entity<EmployeePayroll>()
            .Property(p => p.TotalIncome).HasPrecision(18, 2);
        builder.Entity<EmployeePayroll>()
            .Property(p => p.TotalDeduction).HasPrecision(18, 2);

        builder.Entity<PayrollIncomeItem>()
            .Property(p => p.Amount).HasPrecision(18, 2);
        builder.Entity<PayrollDeductionItem>()
            .Property(p => p.Amount).HasPrecision(18, 2);
        builder.Entity<SalaryGradeIncome>()
            .Property(p => p.Amount).HasPrecision(18, 2);
        builder.Entity<SalaryGradeDeduction>()
            .Property(p => p.Amount).HasPrecision(18, 2);

        builder.Entity<Evaluation>()
            .Property(e => e.FinalScore).HasPrecision(5, 1);
        builder.Entity<Appraisal>()
            .Property(a => a.OverallRating).HasPrecision(5, 2);

        // Employee FK relationships — NoAction to avoid multiple cascade paths
        // (Branch → Department → Employee AND Branch → Employee both lead to Employee)
        builder.Entity<Employee>()
            .HasOne(e => e.Branch).WithMany(b => b.Employees).HasForeignKey(e => e.BranchId).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Employee>()
            .HasOne(e => e.Department).WithMany(d => d.Employees).HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Employee>()
            .HasOne(e => e.Designation).WithMany(d => d.Employees).HasForeignKey(e => e.DesignationId).OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Transfer>()
            .HasOne(t => t.FromBranch).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Transfer>()
            .HasOne(t => t.ToBranch).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Transfer>()
            .HasOne(t => t.FromDepartment).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Transfer>()
            .HasOne(t => t.ToDepartment).WithMany().OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Promotion>()
            .HasOne(p => p.FromDesignation).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Promotion>()
            .HasOne(p => p.ToDesignation).WithMany().OnDelete(DeleteBehavior.NoAction);

        // Global query filter for soft delete
        builder.Entity<Branch>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Department>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Designation>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LeaveCategory>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LeaveRequest>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Evaluation>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Appraisal>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Promotion>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Transfer>().HasQueryFilter(e => !e.IsDeleted);
    }
}
