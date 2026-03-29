using Microsoft.EntityFrameworkCore;
using HRM.Server.Data;
using HRM.Shared.Models;
using HRM.Shared.DTOs;

namespace HRM.Server.Endpoints;

public static class PerformanceEndpoints
{
    public static void MapPerformanceEndpoints(this IEndpointRouteBuilder app)
    {
        MapEvaluationEndpoints(app);
        MapAppraisalEndpoints(app);
        MapPromotionEndpoints(app);
        MapTransferEndpoints(app);
    }

    private static void MapEvaluationEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/evaluations").WithTags("Evaluations").RequireAuthorization();

        group.MapGet("/", async ([AsParameters] PagedRequest req, AppDbContext db) =>
        {
            var query = db.Evaluations.Include(e => e.Employee).AsQueryable();
            if (!string.IsNullOrEmpty(req.Search))
                query = query.Where(e => e.Employee!.FirstName.Contains(req.Search) || e.Employee.LastName.Contains(req.Search));
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(e => e.FinalScore).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync();
            return Results.Ok(ApiResponse<PagedResult<Evaluation>>.Success(new PagedResult<Evaluation> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize }));
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var e = await db.Evaluations.Include(e => e.Employee).FirstOrDefaultAsync(e => e.Id == id);
            return e is null ? Results.NotFound() : Results.Ok(ApiResponse<Evaluation>.Success(e));
        });

        group.MapPost("/", async (Evaluation eval, AppDbContext db) => { db.Evaluations.Add(eval); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Evaluation>.Success(eval)); });
        group.MapPut("/{id:int}", async (int id, Evaluation eval, AppDbContext db) => { var e = await db.Evaluations.FindAsync(id); if (e is null) return Results.NotFound(); db.Entry(e).CurrentValues.SetValues(eval); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Evaluation>.Success(e)); });
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) => { var e = await db.Evaluations.FindAsync(id); if (e is null) return Results.NotFound(); e.IsDeleted = true; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<string>.Success("Deleted")); });
    }

    private static void MapAppraisalEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/appraisals").WithTags("Appraisals").RequireAuthorization();

        group.MapGet("/", async ([AsParameters] PagedRequest req, AppDbContext db) =>
        {
            var query = db.Appraisals.Include(a => a.Employee).AsQueryable();
            if (!string.IsNullOrEmpty(req.Search))
                query = query.Where(a => a.Employee!.FirstName.Contains(req.Search) || a.Employee.LastName.Contains(req.Search));
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(a => a.CreatedAt).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync();
            return Results.Ok(ApiResponse<PagedResult<Appraisal>>.Success(new PagedResult<Appraisal> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize }));
        });

        group.MapPost("/", async (Appraisal a, AppDbContext db) => { db.Appraisals.Add(a); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Appraisal>.Success(a)); });
        group.MapPut("/{id:int}", async (int id, Appraisal a, AppDbContext db) => { var e = await db.Appraisals.FindAsync(id); if (e is null) return Results.NotFound(); db.Entry(e).CurrentValues.SetValues(a); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Appraisal>.Success(e)); });
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) => { var e = await db.Appraisals.FindAsync(id); if (e is null) return Results.NotFound(); e.IsDeleted = true; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<string>.Success("Deleted")); });
    }

    private static void MapPromotionEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/promotions").WithTags("Promotions").RequireAuthorization();

        group.MapGet("/", async ([AsParameters] PagedRequest req, AppDbContext db) =>
        {
            var query = db.Promotions.Include(p => p.Employee).Include(p => p.FromDesignation).Include(p => p.ToDesignation).AsQueryable();
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(p => p.EffectiveDate).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync();
            return Results.Ok(ApiResponse<PagedResult<Promotion>>.Success(new PagedResult<Promotion> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize }));
        });

        group.MapPost("/", async (Promotion p, AppDbContext db) => { db.Promotions.Add(p); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Promotion>.Success(p)); });
        group.MapPut("/{id:int}", async (int id, Promotion p, AppDbContext db) => { var e = await db.Promotions.FindAsync(id); if (e is null) return Results.NotFound(); db.Entry(e).CurrentValues.SetValues(p); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Promotion>.Success(e)); });
    }

    private static void MapTransferEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transfers").WithTags("Transfers").RequireAuthorization();

        group.MapGet("/", async ([AsParameters] PagedRequest req, AppDbContext db) =>
        {
            var query = db.Transfers.Include(t => t.Employee).Include(t => t.FromBranch).Include(t => t.ToBranch).Include(t => t.FromDepartment).Include(t => t.ToDepartment).AsQueryable();
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(t => t.EffectiveDate).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync();
            return Results.Ok(ApiResponse<PagedResult<Transfer>>.Success(new PagedResult<Transfer> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize }));
        });

        group.MapPost("/", async (Transfer t, AppDbContext db) => { db.Transfers.Add(t); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Transfer>.Success(t)); });
        group.MapPut("/{id:int}", async (int id, Transfer t, AppDbContext db) => { var e = await db.Transfers.FindAsync(id); if (e is null) return Results.NotFound(); db.Entry(e).CurrentValues.SetValues(t); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Transfer>.Success(e)); });
    }
}
