using Microsoft.EntityFrameworkCore;
using HRM.Server.Data;
using HRM.Shared.Models;
using HRM.Shared.DTOs;

namespace HRM.Server.Endpoints;

public static class OrganizationEndpoints
{
    public static void MapOrganizationEndpoints(this IEndpointRouteBuilder app)
    {
        MapEmployeeEndpoints(app);
        MapBranchEndpoints(app);
        MapDepartmentEndpoints(app);
        MapDesignationEndpoints(app);
    }

    private static void MapEmployeeEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees").WithTags("Employees").RequireAuthorization();

        group.MapGet("/", async ([AsParameters] PagedRequest req, AppDbContext db) =>
        {
            var query = db.Employees.Include(e => e.Branch).Include(e => e.Department).Include(e => e.Designation).AsQueryable();
            if (!string.IsNullOrEmpty(req.Search))
                query = query.Where(e => e.FirstName.Contains(req.Search) || e.LastName.Contains(req.Search) || e.EmployeeCode.Contains(req.Search));
            var total = await query.CountAsync();
            var items = await query.OrderBy(e => e.EmployeeCode).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync();
            return Results.Ok(ApiResponse<PagedResult<Employee>>.Success(new PagedResult<Employee> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize }));
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var emp = await db.Employees.Include(e => e.Branch).Include(e => e.Department).Include(e => e.Designation).FirstOrDefaultAsync(e => e.Id == id);
            return emp is null ? Results.NotFound() : Results.Ok(ApiResponse<Employee>.Success(emp));
        });

        group.MapPost("/", async (Employee emp, AppDbContext db) =>
        {
            db.Employees.Add(emp); await db.SaveChangesAsync();
            return Results.Ok(ApiResponse<Employee>.Success(emp));
        });

        group.MapPut("/{id:int}", async (int id, Employee emp, AppDbContext db) =>
        {
            var existing = await db.Employees.FindAsync(id);
            if (existing is null) return Results.NotFound();
            db.Entry(existing).CurrentValues.SetValues(emp);
            existing.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(ApiResponse<Employee>.Success(existing));
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var emp = await db.Employees.FindAsync(id);
            if (emp is null) return Results.NotFound();
            emp.IsDeleted = true; await db.SaveChangesAsync();
            return Results.Ok(ApiResponse<string>.Success("Deleted"));
        });
    }

    private static void MapBranchEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/branches").WithTags("Branches").RequireAuthorization();
        group.MapGet("/", async (AppDbContext db) => Results.Ok(ApiResponse<List<Branch>>.Success(await db.Branches.Where(b => b.IsActive).ToListAsync())));
        group.MapGet("/{id:int}", async (int id, AppDbContext db) => { var b = await db.Branches.FindAsync(id); return b is null ? Results.NotFound() : Results.Ok(ApiResponse<Branch>.Success(b)); });
        group.MapPost("/", async (Branch branch, AppDbContext db) => { db.Branches.Add(branch); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Branch>.Success(branch)); });
        group.MapPut("/{id:int}", async (int id, Branch branch, AppDbContext db) => { var b = await db.Branches.FindAsync(id); if (b is null) return Results.NotFound(); db.Entry(b).CurrentValues.SetValues(branch); b.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Branch>.Success(b)); });
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) => { var b = await db.Branches.FindAsync(id); if (b is null) return Results.NotFound(); b.IsDeleted = true; b.IsActive = false; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<string>.Success("Deleted")); });
    }

    private static void MapDepartmentEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/departments").WithTags("Departments").RequireAuthorization();
        group.MapGet("/", async (AppDbContext db) => Results.Ok(ApiResponse<List<Department>>.Success(await db.Departments.Include(d => d.Branch).Where(d => d.IsActive).ToListAsync())));
        group.MapGet("/{id:int}", async (int id, AppDbContext db) => { var d = await db.Departments.Include(d => d.Branch).FirstOrDefaultAsync(d => d.Id == id); return d is null ? Results.NotFound() : Results.Ok(ApiResponse<Department>.Success(d)); });
        group.MapPost("/", async (Department dept, AppDbContext db) => { db.Departments.Add(dept); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Department>.Success(dept)); });
        group.MapPut("/{id:int}", async (int id, Department dept, AppDbContext db) => { var d = await db.Departments.FindAsync(id); if (d is null) return Results.NotFound(); db.Entry(d).CurrentValues.SetValues(dept); d.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Department>.Success(d)); });
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) => { var d = await db.Departments.FindAsync(id); if (d is null) return Results.NotFound(); d.IsDeleted = true; d.IsActive = false; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<string>.Success("Deleted")); });
    }

    private static void MapDesignationEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/designations").WithTags("Designations").RequireAuthorization();
        group.MapGet("/", async (AppDbContext db) => Results.Ok(ApiResponse<List<Designation>>.Success(await db.Designations.Where(d => d.IsActive).ToListAsync())));
        group.MapPost("/", async (Designation desig, AppDbContext db) => { db.Designations.Add(desig); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Designation>.Success(desig)); });
        group.MapPut("/{id:int}", async (int id, Designation desig, AppDbContext db) => { var d = await db.Designations.FindAsync(id); if (d is null) return Results.NotFound(); db.Entry(d).CurrentValues.SetValues(desig); d.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Designation>.Success(d)); });
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) => { var d = await db.Designations.FindAsync(id); if (d is null) return Results.NotFound(); d.IsDeleted = true; d.IsActive = false; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<string>.Success("Deleted")); });
    }
}
