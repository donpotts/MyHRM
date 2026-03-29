using Microsoft.EntityFrameworkCore;
using HRM.Server.Data;
using HRM.Shared.Models;
using HRM.Shared.DTOs;

namespace HRM.Server.Endpoints;

public static class PayrollEndpoints
{
    public static void MapPayrollEndpoints(this IEndpointRouteBuilder app)
    {
        MapSalaryGradeEndpoints(app);
        MapIncomeComponentEndpoints(app);
        MapDeductionComponentEndpoints(app);
        MapPayrollRunEndpoints(app);
    }

    private static void MapSalaryGradeEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/salary-grades").WithTags("Salary Grades").RequireAuthorization();
        group.MapGet("/", async (AppDbContext db) => Results.Ok(ApiResponse<List<SalaryGrade>>.Success(
            await db.SalaryGrades
                .Include(s => s.Incomes).ThenInclude(i => i.IncomeComponent)
                .Include(s => s.Deductions).ThenInclude(d => d.DeductionComponent)
                .Where(s => s.IsActive).ToListAsync())));
        group.MapPost("/", async (SalaryGrade g, AppDbContext db) => { db.SalaryGrades.Add(g); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<SalaryGrade>.Success(g)); });
        group.MapPut("/{id:int}", async (int id, SalaryGrade g, AppDbContext db) => { var e = await db.SalaryGrades.FindAsync(id); if (e is null) return Results.NotFound(); db.Entry(e).CurrentValues.SetValues(g); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<SalaryGrade>.Success(e)); });
    }

    private static void MapIncomeComponentEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/income-components").WithTags("Income Components").RequireAuthorization();
        group.MapGet("/", async (AppDbContext db) => Results.Ok(ApiResponse<List<IncomeComponent>>.Success(await db.IncomeComponents.Where(i => i.IsActive).ToListAsync())));
        group.MapPost("/", async (IncomeComponent c, AppDbContext db) => { db.IncomeComponents.Add(c); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<IncomeComponent>.Success(c)); });
        group.MapPut("/{id:int}", async (int id, IncomeComponent c, AppDbContext db) => { var e = await db.IncomeComponents.FindAsync(id); if (e is null) return Results.NotFound(); db.Entry(e).CurrentValues.SetValues(c); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<IncomeComponent>.Success(e)); });
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) => { var e = await db.IncomeComponents.FindAsync(id); if (e is null) return Results.NotFound(); e.IsActive = false; e.IsDeleted = true; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<string>.Success("Deleted")); });
    }

    private static void MapDeductionComponentEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/deduction-components").WithTags("Deduction Components").RequireAuthorization();
        group.MapGet("/", async (AppDbContext db) => Results.Ok(ApiResponse<List<DeductionComponent>>.Success(await db.DeductionComponents.Where(d => d.IsActive).ToListAsync())));
        group.MapPost("/", async (DeductionComponent c, AppDbContext db) => { db.DeductionComponents.Add(c); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<DeductionComponent>.Success(c)); });
        group.MapPut("/{id:int}", async (int id, DeductionComponent c, AppDbContext db) => { var e = await db.DeductionComponents.FindAsync(id); if (e is null) return Results.NotFound(); db.Entry(e).CurrentValues.SetValues(c); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<DeductionComponent>.Success(e)); });
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) => { var e = await db.DeductionComponents.FindAsync(id); if (e is null) return Results.NotFound(); e.IsActive = false; e.IsDeleted = true; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<string>.Success("Deleted")); });
    }

    private static void MapPayrollRunEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payroll").WithTags("Payroll").RequireAuthorization();

        group.MapGet("/", async ([AsParameters] PagedRequest req, int? month, int? year, AppDbContext db) =>
        {
            var m = month ?? DateTime.Now.Month;
            var y = year ?? DateTime.Now.Year;
            var query = db.EmployeePayrolls.Include(p => p.Employee).Where(p => p.Month == m && p.Year == y);
            if (!string.IsNullOrEmpty(req.Search))
                query = query.Where(p => p.Employee!.FirstName.Contains(req.Search) || p.Employee.LastName.Contains(req.Search) || p.Employee.EmployeeCode.Contains(req.Search));
            var total = await query.CountAsync();
            var items = await query.OrderBy(p => p.Employee!.EmployeeCode).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync();
            return Results.Ok(ApiResponse<PagedResult<EmployeePayroll>>.Success(new PagedResult<EmployeePayroll> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize }));
        });

        group.MapGet("/{id:int}/slip", async (int id, AppDbContext db) =>
        {
            var payroll = await db.EmployeePayrolls
                .Include(p => p.Employee)
                .Include(p => p.IncomeItems).ThenInclude(i => i.IncomeComponent)
                .Include(p => p.DeductionItems).ThenInclude(d => d.DeductionComponent)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (payroll is null) return Results.NotFound();

            var slip = new PayrollSlipDto
            {
                EmployeeName   = payroll.Employee!.FullName,
                EmployeeCode   = payroll.Employee.EmployeeCode,
                Period         = payroll.Period,
                BasicSalary    = payroll.BasicSalary,
                TakeHomePay    = payroll.TakeHomePay,
                TotalIncome    = payroll.TotalIncome,
                TotalDeduction = payroll.TotalDeduction,
                Incomes    = payroll.IncomeItems.Select(i => new PayrollLineItem { Name = i.IncomeComponent?.Name ?? "", Amount = i.Amount }).ToList(),
                Deductions = payroll.DeductionItems.Select(d => new PayrollLineItem { Name = d.DeductionComponent?.Name ?? "", Amount = d.Amount }).ToList()
            };
            return Results.Ok(ApiResponse<PayrollSlipDto>.Success(slip));
        });
    }
}
