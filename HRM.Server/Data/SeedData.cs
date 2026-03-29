using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HRM.Shared.Models;
using HRM.Shared.Enums;

namespace HRM.Server.Data;

public static class SeedData
{
    // ── Called once on startup: creates schema + core reference data only ──
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();
        await SeedCoreAsync(services);
    }

    // ── Core: roles, demo users, currencies, auto-numbers ──
    public static async Task SeedCoreAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Roles
        string[] roles = ["Super Admin", "Admin", "HR Manager", "Employee"];
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        // Demo login accounts
        var demoUsers = new[]
        {
            ("admin@root.com", "admin",  "Super", "Admin",   "Super Admin"),
            ("hr@root.com",    "hr1234", "HR",    "Manager", "HR Manager"),
            ("emp@root.com",   "emp123", "John",  "Employee","Employee"),
        };
        foreach (var (email, pwd, first, last, role) in demoUsers)
        {
            if (await userManager.FindByEmailAsync(email) is null)
            {
                var user = new ApplicationUser
                {
                    UserName = email, Email = email,
                    FirstName = first, LastName = last,
                    EmailConfirmed = true, IsActive = true
                };
                var result = await userManager.CreateAsync(user, pwd);
                if (result.Succeeded) await userManager.AddToRoleAsync(user, role);
            }
        }

        // Currencies (idempotent)
        if (!context.Currencies.Any())
        {
            context.Currencies.AddRange(
                new Currency { Code = "USD", Name = "US Dollar",       Symbol = "$", IsDefault = true },
                new Currency { Code = "EUR", Name = "Euro",             Symbol = "€" },
                new Currency { Code = "GBP", Name = "British Pound",    Symbol = "£" },
                new Currency { Code = "JPY", Name = "Japanese Yen",     Symbol = "¥" }
            );
            await context.SaveChangesAsync();
        }

        // Auto Numbers
        if (!context.AutoNumbers.Any())
        {
            context.AutoNumbers.Add(new AutoNumber { EntityName = "Employee", Prefix = "ACME", LastNumber = 2000, NumberLength = 4 });
            await context.SaveChangesAsync();
        }
    }

    // ── Demo: branches, departments, designations, grades, 30 employees + all related data ──
    public static async Task SeedDemoDataAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Guard: skip if already seeded
        if (await context.Branches.AnyAsync()) return;

        // Branches
        var branches = new List<Branch>
        {
            new() { Name = "New York HQ",    Address = "350 Fifth Avenue",    City = "New York", Country = "USA",       Phone = "+1-212-555-0100", Email = "ny@acme.com" },
            new() { Name = "London Office",  Address = "1 Canada Square",     City = "London",   Country = "UK",        Phone = "+44-20-7946-0958",Email = "london@acme.com" },
            new() { Name = "Tokyo Branch",   Address = "1-1 Marunouchi",      City = "Tokyo",    Country = "Japan",     Phone = "+81-3-1234-5678", Email = "tokyo@acme.com" },
            new() { Name = "Sydney Office",  Address = "200 George St",       City = "Sydney",   Country = "Australia", Phone = "+61-2-9876-5432", Email = "sydney@acme.com" },
            new() { Name = "Berlin Hub",     Address = "Unter den Linden 77", City = "Berlin",   Country = "Germany",   Phone = "+49-30-1234-5678",Email = "berlin@acme.com" },
        };
        context.Branches.AddRange(branches);
        await context.SaveChangesAsync();

        // Departments
        var departments = new List<Department>
        {
            new() { Name = "Engineering",    BranchId = branches[0].Id, Description = "Software development & IT" },
            new() { Name = "Operations",     BranchId = branches[0].Id, Description = "Business operations & logistics" },
            new() { Name = "Human Resources",BranchId = branches[0].Id, Description = "People management & culture" },
            new() { Name = "Finance",        BranchId = branches[1].Id, Description = "Financial planning & accounting" },
            new() { Name = "Marketing",      BranchId = branches[1].Id, Description = "Brand & growth marketing" },
        };
        context.Departments.AddRange(departments);
        await context.SaveChangesAsync();

        // Designations
        var designations = new List<Designation>
        {
            new() { Name = "Junior Developer",      Level = 1 },
            new() { Name = "Senior Developer",      Level = 2 },
            new() { Name = "Team Lead",             Level = 3 },
            new() { Name = "Manager",               Level = 4 },
            new() { Name = "Director",              Level = 5 },
            new() { Name = "VP",                    Level = 6 },
            new() { Name = "Analyst",               Level = 2 },
            new() { Name = "Senior Analyst",        Level = 3 },
            new() { Name = "HR Specialist",         Level = 2 },
            new() { Name = "Marketing Coordinator", Level = 1 },
        };
        context.Designations.AddRange(designations);

        // Leave Categories
        var leaveCategories = new List<LeaveCategory>
        {
            new() { Name = "Annual Leave",    Description = "Paid annual vacation",      MaxDaysPerYear = 20 },
            new() { Name = "Sick Leave",      Description = "Paid sick days",            MaxDaysPerYear = 10 },
            new() { Name = "Personal Leave",  Description = "Personal days off",         MaxDaysPerYear = 5  },
            new() { Name = "Maternity Leave", Description = "Maternity/paternity leave", MaxDaysPerYear = 90 },
            new() { Name = "Unpaid Leave",    Description = "Leave without pay",         MaxDaysPerYear = 30 },
        };
        context.LeaveCategories.AddRange(leaveCategories);

        // Salary Grades
        var grades = new List<SalaryGrade>
        {
            new() { Name = "Grade A", Description = "Entry level",  BasicSalary = 5000  },
            new() { Name = "Grade B", Description = "Mid level",    BasicSalary = 7500  },
            new() { Name = "Grade C", Description = "Senior level", BasicSalary = 10000 },
            new() { Name = "Grade D", Description = "Management",   BasicSalary = 13000 },
            new() { Name = "Grade E", Description = "Executive",    BasicSalary = 18000 },
        };
        context.SalaryGrades.AddRange(grades);

        // Income / Deduction Components
        var incomes = new List<IncomeComponent>
        {
            new() { Name = "WFH Stipend",        Description = "Work from home allowance" },
            new() { Name = "Transport Allowance", Description = "Monthly transport" },
            new() { Name = "Meal Allowance",      Description = "Daily meal subsidy" },
            new() { Name = "Performance Bonus",   Description = "Quarterly bonus" },
        };
        var deductions = new List<DeductionComponent>
        {
            new() { Name = "Health Insurance Premium", Description = "Medical insurance" },
            new() { Name = "401(k) Contribution",      Description = "Retirement savings" },
            new() { Name = "Tax Withholding",          Description = "Income tax" },
            new() { Name = "Life Insurance",           Description = "Life insurance premium" },
        };
        context.IncomeComponents.AddRange(incomes);
        context.DeductionComponents.AddRange(deductions);
        await context.SaveChangesAsync();

        // Salary Grade <-> Income/Deduction mappings
        var gradeIncomes    = new List<SalaryGradeIncome>();
        var gradeDeductions = new List<SalaryGradeDeduction>();
        foreach (var g in grades)
        {
            gradeIncomes.Add(new() { SalaryGradeId = g.Id, IncomeComponentId = incomes[0].Id, Amount = 150 });
            gradeDeductions.Add(new() { SalaryGradeId = g.Id, DeductionComponentId = deductions[0].Id, Amount = 250 });
            gradeDeductions.Add(new() { SalaryGradeId = g.Id, DeductionComponentId = deductions[1].Id, Amount = g.BasicSalary * 0.05m });
        }
        context.SalaryGradeIncomes.AddRange(gradeIncomes);
        context.SalaryGradeDeductions.AddRange(gradeDeductions);
        await context.SaveChangesAsync();

        // 30 Employees
        var firstNames = new[] { "Jennifer","Michael","Emily","David","Sarah","Robert","Linda","Thomas","Patricia","Charles",
            "Elizabeth","Joseph","Margaret","Richard","Susan","Daniel","Jessica","Matthew","Karen","Andrew",
            "Nancy","James","Betty","Christopher","Dorothy","William","Lisa","Mark","Helen","George" };
        var lastNames = new[] { "Perez","Johnson","Davis","Wilson","Brown","Taylor","Anderson","Martin","Smith","White",
            "Harris","Hernandez","Clark","Moore","Lewis","Robinson","Walker","Young","Allen","King",
            "Wright","Scott","Green","Adams","Baker","Hall","Rivera","Campbell","Mitchell","Garcia" };

        var autoNum = await context.AutoNumbers.FirstAsync(a => a.EntityName == "Employee");
        var employees = new List<Employee>();
        for (int i = 0; i < 30; i++)
        {
            autoNum.LastNumber++;
            employees.Add(new Employee
            {
                EmployeeCode  = $"ACME-{autoNum.LastNumber:D4}",
                FirstName     = firstNames[i],
                LastName      = lastNames[i],
                Email         = $"{firstNames[i].ToLower()}.{lastNames[i].ToLower()}@acme.com",
                Phone         = $"+1-555-{1000+i:D4}",
                DateOfBirth   = new DateTime(1980+(i%15), (i%12)+1, (i%28)+1),
                HireDate      = new DateTime(2020+(i%5), (i%12)+1, 1),
                Gender        = i%2==0 ? Gender.Female : Gender.Male,
                Status        = EmploymentStatus.Active,
                BranchId      = branches[i%5].Id,
                DepartmentId  = departments[i%5].Id,
                DesignationId = designations[i%10].Id,
                SalaryGradeId = grades[i%5].Id,
                FTE           = 1.00m,
                City          = new[]{"New York","London","Tokyo","Sydney","Berlin"}[i%5],
                Country       = new[]{"USA","UK","Japan","Australia","Germany"}[i%5],
            });
        }
        context.Employees.AddRange(employees);
        await context.SaveChangesAsync();

        // Evaluations (20 of 30 employees)
        context.Evaluations.AddRange(employees.Take(20).Select((emp, i) => new Evaluation
        {
            EmployeeId     = emp.Id,
            Period         = "Annual 2025 Review",
            FinalScore     = 85 + (i%16),
            Rating         = 3 + (i%3),
            Evaluator      = employees[(i+5)%20].FullName,
            Status         = EvaluationStatus.Completed,
            CompletedDate  = new DateTime(2025, 12, 15),
        }));

        // Leave Requests
        context.LeaveRequests.AddRange(employees.Take(10).Select((emp, i) => new LeaveRequest
        {
            EmployeeId       = emp.Id,
            LeaveCategoryId  = leaveCategories[i%3].Id,
            StartDate        = DateTime.Today.AddDays(i*3),
            EndDate          = DateTime.Today.AddDays(i*3+2),
            TotalDays        = 3,
            Reason           = "Personal time off",
            Status           = i < 5 ? LeaveStatus.Pending : LeaveStatus.Approved,
        }));

        // Leave Balances
        context.LeaveBalances.AddRange(employees.SelectMany(emp =>
            leaveCategories.Select(cat => new LeaveBalance
            {
                EmployeeId      = emp.Id,
                LeaveCategoryId = cat.Id,
                Year            = 2026,
                TotalEntitled   = cat.MaxDaysPerYear,
                Used            = Random.Shared.Next(0, Math.Min(5, cat.MaxDaysPerYear)),
            })
        ));

        // Transfer
        context.Transfers.Add(new Transfer
        {
            EmployeeId       = employees[6].Id,
            FromBranchId     = branches[1].Id,
            ToBranchId       = branches[0].Id,
            FromDepartmentId = departments[1].Id,
            ToDepartmentId   = departments[0].Id,
            EffectiveDate    = DateTime.Today.AddDays(30),
            PipelineStage    = "In Progress",
            Status           = TransferStatus.Approved,
            Reason           = "Team restructuring",
        });

        await context.SaveChangesAsync();

        // Payrolls (March 2026)
        var payrolls = employees.Select(emp =>
        {
            var g   = grades.First(g => g.Id == emp.SalaryGradeId);
            var gi  = gradeIncomes.Where(x => x.SalaryGradeId == g.Id).ToList();
            var gd  = gradeDeductions.Where(x => x.SalaryGradeId == g.Id).ToList();
            return new EmployeePayroll
            {
                EmployeeId     = emp.Id,
                Month          = 3, Year = 2026,
                BasicSalary    = g.BasicSalary,
                TotalIncome    = gi.Sum(x => x.Amount),
                TotalDeduction = gd.Sum(x => x.Amount),
                Status         = PayrollStatus.Processed,
                IncomeItems    = gi.Select(x => new PayrollIncomeItem    { IncomeComponentId    = x.IncomeComponentId,    Amount = x.Amount }).ToList(),
                DeductionItems = gd.Select(x => new PayrollDeductionItem { DeductionComponentId = x.DeductionComponentId, Amount = x.Amount }).ToList(),
            };
        }).ToList();
        context.EmployeePayrolls.AddRange(payrolls);
        await context.SaveChangesAsync();
    }

    // ── Reset: drop, recreate, seed core only ──
    public static async Task ResetAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await SeedCoreAsync(services);
    }

    // ── Reset + Demo: drop, recreate, seed everything ──
    public static async Task ResetWithDemoAsync(IServiceProvider services)
    {
        await ResetAsync(services);
        await SeedDemoDataAsync(services);
    }
}
