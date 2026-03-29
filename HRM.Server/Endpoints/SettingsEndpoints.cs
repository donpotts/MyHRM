using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HRM.Server.Data;
using HRM.Shared.Models;
using HRM.Shared.DTOs;

namespace HRM.Server.Endpoints;

public static class SettingsEndpoints
{
    public static void MapSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        MapCurrencyEndpoints(app);
        MapAutoNumberEndpoints(app);
        MapUsersEndpoints(app);
        MapSystemLogsEndpoints(app);
        MapSystemSettingsEndpoints(app);
        MapBrandingEndpoints(app);
    }

    private static void MapCurrencyEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/currencies").WithTags("Currencies").RequireAuthorization();
        group.MapGet("/", async (AppDbContext db) => Results.Ok(ApiResponse<List<Currency>>.Success(await db.Currencies.Where(c => c.IsActive).ToListAsync())));
        group.MapPost("/", async (Currency c, AppDbContext db) => { db.Currencies.Add(c); await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Currency>.Success(c)); });
        group.MapPut("/{id:int}", async (int id, Currency c, AppDbContext db) => { var e = await db.Currencies.FindAsync(id); if (e is null) return Results.NotFound(); db.Entry(e).CurrentValues.SetValues(c); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<Currency>.Success(e)); });
    }

    private static void MapAutoNumberEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auto-numbers").WithTags("Auto Numbers").RequireAuthorization();
        group.MapGet("/", async (AppDbContext db) => Results.Ok(ApiResponse<List<AutoNumber>>.Success(await db.AutoNumbers.ToListAsync())));
        group.MapPut("/{id:int}", async (int id, AutoNumber a, AppDbContext db) => { var e = await db.AutoNumbers.FindAsync(id); if (e is null) return Results.NotFound(); db.Entry(e).CurrentValues.SetValues(a); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); return Results.Ok(ApiResponse<AutoNumber>.Success(e)); });
    }

    private static void MapUsersEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async (UserManager<ApplicationUser> userManager) =>
        {
            var users = userManager.Users.Where(u => u.IsActive).ToList();
            var result = new List<UserInfo>();
            foreach (var u in users)
            {
                var roles = await userManager.GetRolesAsync(u);
                result.Add(new UserInfo { Id = u.Id, Email = u.Email!, FirstName = u.FirstName, LastName = u.LastName, AvatarUrl = u.AvatarUrl, Role = roles.FirstOrDefault() });
            }
            return Results.Ok(ApiResponse<List<UserInfo>>.Success(result));
        }).WithTags("Users").RequireAuthorization();
    }

    private static void MapSystemLogsEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/system-logs").WithTags("System Logs").RequireAuthorization()
            .MapGet("/", async ([AsParameters] PagedRequest req, AppDbContext db) =>
            {
                var query = db.SystemLogs.AsQueryable();
                if (!string.IsNullOrEmpty(req.Search))
                    query = query.Where(l => l.Message.Contains(req.Search));
                var total = await query.CountAsync();
                var items = await query.OrderByDescending(l => l.Timestamp).Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync();
                return Results.Ok(ApiResponse<PagedResult<SystemLog>>.Success(new PagedResult<SystemLog> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize }));
            });
    }

    private static void MapBrandingEndpoints(IEndpointRouteBuilder app)
    {
        const string brandingFile = "branding.json";
        string[] allowedTypes = ["logo", "favicon"];
        string[] allowedExt   = [".png", ".jpg", ".jpeg", ".ico", ".svg", ".gif"];

        app.MapGet("/api/branding", async (IWebHostEnvironment env) =>
        {
            var path = Path.Combine(env.ContentRootPath, brandingFile);
            if (!File.Exists(path))
                return Results.Ok(ApiResponse<CompanyBrandingDto>.Success(new CompanyBrandingDto()));
            var json = await File.ReadAllTextAsync(path);
            var dto = System.Text.Json.JsonSerializer.Deserialize<CompanyBrandingDto>(json) ?? new();
            return Results.Ok(ApiResponse<CompanyBrandingDto>.Success(dto));
        }).WithTags("Branding");

        app.MapPut("/api/branding", async (CompanyBrandingDto dto, IWebHostEnvironment env) =>
        {
            var path = Path.Combine(env.ContentRootPath, brandingFile);
            var json = System.Text.Json.JsonSerializer.Serialize(dto,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
            return Results.Ok(ApiResponse<CompanyBrandingDto>.Success(dto));
        }).WithTags("Branding").RequireAuthorization();

        app.MapPost("/api/branding/upload/{type}", async (string type, HttpRequest request, IWebHostEnvironment env) =>
        {
            if (!allowedTypes.Contains(type))
                return Results.BadRequest(ApiResponse<string>.Fail("Invalid asset type. Use: logo, favicon"));

            var form = await request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();
            if (file is null || file.Length == 0)
                return Results.BadRequest(ApiResponse<string>.Fail("No file uploaded"));

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext))
                return Results.BadRequest(ApiResponse<string>.Fail("Invalid file type. Allowed: PNG, JPG, ICO, SVG, GIF"));
            if (file.Length > 2 * 1024 * 1024)
                return Results.BadRequest(ApiResponse<string>.Fail("File too large (max 2MB)"));

            var brandingDir = Path.Combine(env.WebRootPath ?? "wwwroot", "branding");
            Directory.CreateDirectory(brandingDir);

            // Remove previous files of same type
            foreach (var old in Directory.GetFiles(brandingDir, $"{type}.*"))
                File.Delete(old);

            var fileName = $"{type}{ext}";
            var filePath = Path.Combine(brandingDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var url = $"/branding/{fileName}?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

            // Patch branding.json
            var jsonPath = Path.Combine(env.ContentRootPath, brandingFile);
            var branding = File.Exists(jsonPath)
                ? System.Text.Json.JsonSerializer.Deserialize<CompanyBrandingDto>(await File.ReadAllTextAsync(jsonPath)) ?? new()
                : new CompanyBrandingDto();

            if (type == "logo")    branding.LogoUrl    = url;
            if (type == "favicon") branding.FaviconUrl = url;

            await File.WriteAllTextAsync(jsonPath, System.Text.Json.JsonSerializer.Serialize(branding,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

            return Results.Ok(ApiResponse<string>.Success(url));
        }).WithTags("Branding").RequireAuthorization().DisableAntiforgery();
    }

    private static void MapSystemSettingsEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/system-settings", (IConfiguration config) =>
        {
            var settings = new SystemSettingsDto
            {
                Identity = new IdentitySettings
                {
                    RequireDigit             = config.GetValue<bool>("Identity:RequireDigit"),
                    RequiredLength           = config.GetValue<int>("Identity:RequiredLength"),
                    RequireNonAlphanumeric   = config.GetValue<bool>("Identity:RequireNonAlphanumeric"),
                    RequireUppercase         = config.GetValue<bool>("Identity:RequireUppercase"),
                    RequireLowercase         = config.GetValue<bool>("Identity:RequireLowercase"),
                    RequiredUniqueChars      = config.GetValue<int>("Identity:RequiredUniqueChars"),
                    CookieName               = config["Cookie:Name"] ?? "",
                    LoginPath                = config["Cookie:LoginPath"] ?? "",
                    LogoutPath               = config["Cookie:LogoutPath"] ?? "",
                    AccessDeniedPath         = config["Cookie:AccessDeniedPath"] ?? "",
                    ExpireDays               = config.GetValue<int>("Cookie:ExpireDays"),
                    DefaultAdminEmail        = config["DefaultAdmin:Email"] ?? "",
                    DefaultAdminPassword     = "********"
                },
                Database = new DatabaseSettings { Provider = "SQL Server", Server = "(localdb)\\MSSQLLocalDB", DatabaseName = "HRM" },
                Jobs = new JobSettings
                {
                    EnableBackgroundJobs   = config.GetValue<bool>("Jobs:EnableBackgroundJobs"),
                    PayrollProcessingDay   = config.GetValue<int>("Jobs:PayrollProcessingDay")
                },
                Email = new EmailSettings
                {
                    SmtpHost    = config["Email:SmtpHost"] ?? "",
                    SmtpPort    = config.GetValue<int>("Email:SmtpPort"),
                    FromAddress = config["Email:FromAddress"] ?? "",
                    EnableSsl   = config.GetValue<bool>("Email:EnableSsl")
                },
                Storage = new StorageSettings
                {
                    AvatarPath          = config["Storage:AvatarPath"] ?? "",
                    MaxFileSize         = config.GetValue<long>("Storage:MaxFileSize"),
                    AllowedExtensions   = config["Storage:AllowedExtensions"] ?? ""
                },
                Logging = new LoggingSettings
                {
                    MinimumLevel          = config["Serilog:MinimumLevel:Default"] ?? "Information",
                    EnableDatabaseLogging = true,
                    EnableFileLogging     = true,
                    LogFilePath           = "Logs/hrm-.log"
                }
            };
            return Results.Ok(ApiResponse<SystemSettingsDto>.Success(settings));
        }).WithTags("System Settings").RequireAuthorization();
    }
}
