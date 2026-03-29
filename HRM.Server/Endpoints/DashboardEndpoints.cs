using Microsoft.EntityFrameworkCore;
using HRM.Server.Data;
using HRM.Shared.DTOs;
using HRM.Shared.Enums;

namespace HRM.Server.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard", async (AppDbContext db) =>
        {
            var totalStaff  = await db.Employees.CountAsync(e => e.Status == EmploymentStatus.Active);
            var avgFTE      = totalStaff > 0 ? await db.Employees.Where(e => e.Status == EmploymentStatus.Active).AverageAsync(e => e.FTE) : 0;
            var branches    = await db.Branches.CountAsync(b => b.IsActive);
            var departments = await db.Departments.CountAsync(d => d.IsActive);

            var activeTransfers = await db.Transfers
                .Where(t => t.Status == TransferStatus.Approved)
                .Include(t => t.Employee).Include(t => t.ToBranch)
                .Select(t => new ActiveTransferDto
                {
                    EmployeeName  = t.Employee!.FirstName,
                    PipelineStage = t.PipelineStage ?? "Pending",
                    Location      = t.ToBranch!.Name
                }).ToListAsync();

            var pendingLeaves = await db.LeaveRequests
                .Where(l => l.Status == LeaveStatus.Pending)
                .Select(l => new PendingLeaveDto
                {
                    RequestId = $"LR-{l.Id:D4}",
                    Status    = l.Status.ToString(),
                    DueDate   = l.StartDate
                }).Take(5).ToListAsync();

            var lowPerf = await db.Evaluations.CountAsync(e => e.FinalScore < 70);

            return Results.Ok(ApiResponse<DashboardData>.Success(new DashboardData
            {
                TotalStaff          = totalStaff,
                AverageFTE          = avgFTE,
                AverageCostPerHire  = 11600,
                RetentionRate       = 94.8m,
                LeaversRate         = 5.2m,
                UtilizationRate     = 82.5m,
                EngagementRate      = 74.2m,
                OperationalBranches = branches,
                Departments         = departments,
                Pipeline    = new RecruitmentPipeline { Sourced = 124, Interviewed = 38, Offered = 12 },
                Experience  = new EmployeeExperience { NPS = 8.5m, Satisfaction = 78 },
                Forecast    = new HeadcountForecast { ProjectedFTE = 5, FocusDepartments = "Engineering & Operations", TimeToHire = 24 },
                Talent      = new TalentScoring { TopTalent = 0, CorePlayer = 0, LowPerf = lowPerf > 0 ? lowPerf : 10, CertifiedPercent = 94 },
                ActiveTransfers = activeTransfers,
                PendingLeaves   = pendingLeaves
            }));
        }).WithTags("Dashboard").RequireAuthorization();
    }
}
