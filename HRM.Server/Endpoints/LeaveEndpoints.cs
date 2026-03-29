using Microsoft.EntityFrameworkCore;
using HRM.Server.Data;
using HRM.Shared.Models;
using HRM.Shared.DTOs;
using HRM.Shared.Enums;

namespace HRM.Server.Endpoints;

public static class LeaveEndpoints
{
    public static void MapLeaveEndpoints(this IEndpointRouteBuilder app)
    {
        MapLeaveCategoryEndpoints(app);
        MapLeaveRequestEndpoints(app);
        MapLeaveBalanceEndpoints(app);
    }

    private static void MapLeaveCategoryEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/leave-categories").WithTags("Leave Categories").RequireAuthorization();
        group.MapGet("/", async (AppDbContext db) => Results.Ok(ApiResponse<List<LeaveCategory>>.Success(await db.LeaveCategories.Where(c => c.IsActive).ToListAsync())));
        group.MapPost("/", async (LeaveCategory cat, AppDbContext db) => { db.LeaveCategories.Add(cat); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<LeaveCategory>.Success(cat)); });
        group.MapPut("/{id:int}", async (int id, LeaveCategory cat, AppDbContext db) => { var c = await db.LeaveCategories.FindAsync(id); if (c is null) return Results.NotFound(); db.Entry(c).CurrentValues.SetValues(cat); c.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<LeaveCategory>.Success(c)); });
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) => { var c = await db.LeaveCategories.FindAsync(id); if (c is null) return Results.NotFound(); c.IsDeleted = true; c.IsActive = false; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<string>.Success("Deleted")); });
    }

    private static void MapLeaveRequestEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/leave-requests").WithTags("Leave Requests").RequireAuthorization();

        group.MapGet("/", async ([AsParameters] PagedRequest req, AppDbContext db) =>
        {
            var query = db.LeaveRequests.Include(l => l.Employee).Include(l => l.LeaveCategory).AsQueryable();
            if (!string.IsNullOrEmpty(req.Search))
                query = query.Where(l => l.Employee!.FirstName.Contains(req.Search) || l.Employee.LastName.Contains(req.Search));
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(l => l.CreatedAt).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync();
            return Results.Ok(ApiResponse<PagedResult<LeaveRequest>>.Success(new PagedResult<LeaveRequest> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize }));
        });

        group.MapPost("/", async (LeaveRequest req, AppDbContext db) =>
        {
            db.LeaveRequests.Add(req); await db.SaveChangesAsync();
            return Results.Ok(ApiResponse<LeaveRequest>.Success(req));
        });

        group.MapPut("/{id:int}/approve", async (int id, AppDbContext db) =>
        {
            var lr = await db.LeaveRequests.FindAsync(id);
            if (lr is null) return Results.NotFound();
            lr.Status = LeaveStatus.Approved; lr.ApprovedDate = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(ApiResponse<LeaveRequest>.Success(lr));
        });

        group.MapPut("/{id:int}/reject", async (int id, AppDbContext db) =>
        {
            var lr = await db.LeaveRequests.FindAsync(id);
            if (lr is null) return Results.NotFound();
            lr.Status = LeaveStatus.Rejected;
            await db.SaveChangesAsync();
            return Results.Ok(ApiResponse<LeaveRequest>.Success(lr));
        });
    }

    private static void MapLeaveBalanceEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/leave-balances/{employeeId:int}", async (int employeeId, AppDbContext db) =>
        {
            var balances = await db.LeaveBalances
                .Include(b => b.LeaveCategory)
                .Where(b => b.EmployeeId == employeeId && b.Year == DateTime.Now.Year)
                .ToListAsync();
            return Results.Ok(ApiResponse<List<LeaveBalance>>.Success(balances));
        }).WithTags("Leave Balances").RequireAuthorization();
    }
}
