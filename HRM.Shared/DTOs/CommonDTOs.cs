namespace HRM.Shared.DTOs;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string? message = null) =>
        new() { IsSuccess = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message) =>
        new() { IsSuccess = false, Message = message };
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool? SortDescending { get; set; }
}

public class DashboardData
{
    public int TotalStaff { get; set; }
    public decimal AverageFTE { get; set; }
    public decimal AverageCostPerHire { get; set; }
    public decimal RetentionRate { get; set; }
    public decimal LeaversRate { get; set; }
    public decimal UtilizationRate { get; set; }
    public decimal EngagementRate { get; set; }
    public int OperationalBranches { get; set; }
    public int Departments { get; set; }
    public RecruitmentPipeline Pipeline { get; set; } = new();
    public EmployeeExperience Experience { get; set; } = new();
    public HeadcountForecast Forecast { get; set; } = new();
    public TalentScoring Talent { get; set; } = new();
    public List<ActiveTransferDto> ActiveTransfers { get; set; } = [];
    public List<PendingLeaveDto> PendingLeaves { get; set; } = [];
}

public class RecruitmentPipeline
{
    public int Sourced { get; set; }
    public int Interviewed { get; set; }
    public int Offered { get; set; }
}

public class EmployeeExperience
{
    public decimal NPS { get; set; }
    public decimal Satisfaction { get; set; }
}

public class HeadcountForecast
{
    public int ProjectedFTE { get; set; }
    public string FocusDepartments { get; set; } = string.Empty;
    public int TimeToHire { get; set; }
}

public class TalentScoring
{
    public int TopTalent { get; set; }
    public int CorePlayer { get; set; }
    public int LowPerf { get; set; }
    public decimal CertifiedPercent { get; set; }
}

public class ActiveTransferDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string PipelineStage { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public class PendingLeaveDto
{
    public string RequestId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}

public class PayrollSlipDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal TakeHomePay { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalDeduction { get; set; }
    public List<PayrollLineItem> Incomes { get; set; } = [];
    public List<PayrollLineItem> Deductions { get; set; } = [];
}

public class PayrollLineItem
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class SystemSettingsDto
{
    public IdentitySettings Identity { get; set; } = new();
    public DatabaseSettings Database { get; set; } = new();
    public JobSettings Jobs { get; set; } = new();
    public EmailSettings Email { get; set; } = new();
    public StorageSettings Storage { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
}

public class IdentitySettings
{
    public bool RequireDigit { get; set; }
    public int RequiredLength { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireLowercase { get; set; }
    public int RequiredUniqueChars { get; set; }
    public string CookieName { get; set; } = string.Empty;
    public string LoginPath { get; set; } = string.Empty;
    public string LogoutPath { get; set; } = string.Empty;
    public string AccessDeniedPath { get; set; } = string.Empty;
    public int ExpireDays { get; set; }
    public string DefaultAdminEmail { get; set; } = string.Empty;
    public string DefaultAdminPassword { get; set; } = string.Empty;
}

public class DatabaseSettings
{
    public string Provider { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

public class JobSettings
{
    public bool EnableBackgroundJobs { get; set; }
    public int PayrollProcessingDay { get; set; }
}

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
}

public class StorageSettings
{
    public string AvatarPath { get; set; } = string.Empty;
    public long MaxFileSize { get; set; }
    public string AllowedExtensions { get; set; } = string.Empty;
}

public class SetupStatusDto
{
    public bool IsNew { get; set; }         // true when no branches exist
    public bool HasDemoData { get; set; }   // true when employees exist
    public int EmployeeCount { get; set; }
    public int BranchCount { get; set; }
}

public class NewCompanyRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string HeadquartersCity { get; set; } = string.Empty;
    public string HeadquartersCountry { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = "USD";
}

public class LoggingSettings
{
    public string MinimumLevel { get; set; } = string.Empty;
    public bool EnableDatabaseLogging { get; set; }
    public bool EnableFileLogging { get; set; }
    public string LogFilePath { get; set; } = string.Empty;
}

public class CompanyBrandingDto
{
    public string CompanyName    { get; set; } = "HRM";
    public string CompanyTagline { get; set; } = "Human Resources";
    public string LogoText       { get; set; } = "HR";
    public string? LogoUrl       { get; set; }
    public string? FaviconUrl    { get; set; }
    public string AppTitle       { get; set; } = "Human Resource Management";
}
