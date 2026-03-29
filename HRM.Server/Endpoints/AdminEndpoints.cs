using Microsoft.EntityFrameworkCore;
using HRM.Server.Data;
using HRM.Shared.Models;
using HRM.Shared.DTOs;

namespace HRM.Server.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin Setup");

        group.MapGet("/status", async (AppDbContext db) =>
        {
            var branchCount   = await db.Branches.CountAsync();
            var employeeCount = await db.Employees.CountAsync();
            return Results.Ok(ApiResponse<SetupStatusDto>.Success(new SetupStatusDto
            {
                IsNew         = branchCount == 0,
                HasDemoData   = employeeCount > 0,
                EmployeeCount = employeeCount,
                BranchCount   = branchCount,
            }));
        });

        group.MapPost("/load-demo", async (IServiceProvider sp) =>
        {
            await SeedData.SeedDemoDataAsync(sp);
            return Results.Ok(ApiResponse<string>.Success("Demo data loaded successfully."));
        }).RequireAuthorization();

        group.MapPost("/reset", async (IServiceProvider sp) =>
        {
            await SeedData.ResetAsync(sp);
            return Results.Ok(ApiResponse<string>.Success("Database reset to clean state."));
        }).RequireAuthorization();

        group.MapPost("/reset-demo", async (IServiceProvider sp) =>
        {
            await SeedData.ResetWithDemoAsync(sp);
            return Results.Ok(ApiResponse<string>.Success("Database reset and demo data loaded."));
        }).RequireAuthorization();

        group.MapPost("/new-company", async (NewCompanyRequest req, AppDbContext db) =>
        {
            if (await db.Branches.AnyAsync())
                return Results.Ok(ApiResponse<string>.Fail("Company already configured."));

            var hq = new Branch
            {
                Name    = $"{req.CompanyName} HQ",
                Address = "1 Main Street",
                City    = req.HeadquartersCity,
                Country = req.HeadquartersCountry,
                IsActive = true,
            };
            db.Branches.Add(hq);
            await db.SaveChangesAsync();

            db.Departments.Add(new Department { Name = "General", BranchId = hq.Id, Description = "Default department", IsActive = true });

            if (!await db.LeaveCategories.AnyAsync())
                db.LeaveCategories.AddRange(
                    new LeaveCategory { Name = "Annual Leave",   MaxDaysPerYear = 20 },
                    new LeaveCategory { Name = "Sick Leave",     MaxDaysPerYear = 10 },
                    new LeaveCategory { Name = "Personal Leave", MaxDaysPerYear = 5  }
                );
            if (!await db.Designations.AnyAsync())
                db.Designations.AddRange(
                    new Designation { Name = "Staff",   Level = 1 },
                    new Designation { Name = "Manager", Level = 2 }
                );
            if (!await db.SalaryGrades.AnyAsync())
                db.SalaryGrades.Add(new SalaryGrade { Name = "Standard", BasicSalary = 5000 });

            await db.SaveChangesAsync();
            return Results.Ok(ApiResponse<string>.Success("Company configured successfully."));
        }).RequireAuthorization();
    }
}
