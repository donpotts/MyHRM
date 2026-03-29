<div align="center">

# 🏢 HRM — Human Resource Management

**A modern, full-stack HR platform built with .NET 10 and Blazor WebAssembly**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com)
[![Blazor WASM](https://img.shields.io/badge/Blazor-WebAssembly-7B2FBE?style=for-the-badge&logo=blazor)](https://blazor.net)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind-3.x-06B6D4?style=for-the-badge&logo=tailwindcss)](https://tailwindcss.com)
[![SQL Server](https://img.shields.io/badge/SQL_Server-LocalDB-CC2927?style=for-the-badge&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)

---

</div>

## ✨ Overview

HRM is a **white-label, production-ready** Human Resource Management system featuring a responsive Blazor WebAssembly frontend backed by a minimal-API ASP.NET Core server. Everything from organization structure to payroll, leave management, performance reviews, and company branding is built in — with a clean, dark-mode capable UI and a fully dynamic theme engine.

### What makes it different

| Feature | Detail |
|---|---|
| 🎨 **White-label ready** | Custom company name, logo, favicon, and tagline — all live-editable |
| 🌗 **Dark mode + Accent themes** | 6 accent colors × light/dark, applied globally via CSS variables |
| 📱 **Mobile-first** | Every screen is responsive; hamburger nav on mobile, collapsible sidebar on desktop |
| ⚡ **Zero-config demo** | One click loads 30 employees, payroll, evaluations, leave history |
| 🔐 **JWT auth** | Role-based access (Super Admin / HR Manager / Employee) |
| 📊 **Serilog** | Structured logging to file and SQL database |

---

## 🗂️ Project Structure

```
HRM/
├── HRM.Server/          # ASP.NET Core 10 Minimal API backend
│   ├── Data/            # DbContext + seed data
│   ├── Endpoints/       # Organized endpoint groups
│   └── branding.json    # Live white-label config (auto-generated)
│
├── HRM.Client/          # Blazor WebAssembly frontend
│   ├── Layout/          # MainLayout, EmptyLayout
│   ├── Pages/           # Feature pages (see below)
│   ├── Services/        # ApiService, AuthService, ThemeService, BrandingService
│   ├── Shared/          # Reusable components (Modal, Pagination, TabContainer…)
│   └── Styles/          # Tailwind input + PostCSS pipeline
│
└── HRM.Shared/          # Shared models, DTOs, and enums
    ├── Models/          # EF Core entity models
    ├── DTOs/            # Request / response objects
    └── Enums/           # Status codes and typed enums
```

---

## 🚀 Quick Start

### Prerequisites

| Tool | Version | Notes |
|---|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0+ | Required |
| [Node.js](https://nodejs.org) | 18+ | For Tailwind CSS build |
| [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) | Any | Ships with VS 2022 |

### 1 — Clone & restore

```bash
git clone https://github.com/your-org/hrm.git
cd hrm
dotnet restore
```

### 2 — Build Tailwind CSS

```bash
cd HRM.Client
npm install
npm run build:css   # generates wwwroot/css/tailwind.css
cd ..
```

> **Tip:** `npm run watch:css` keeps Tailwind rebuilding automatically while you develop.

### 3 — Configure the connection string

Open `HRM.Server/appsettings.json` and verify (or update) the `DefaultConnection` string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HRM;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

No manual migrations needed — the app creates and seeds the database on first run.

### 4 — Run

```bash
dotnet run --project HRM.Server/HRM.Server.csproj --launch-profile https
```

Open **https://localhost:7136** in your browser. On first launch you'll be guided through the Setup Wizard.

> **Note:** The Tailwind CSS build is also wired into the MSBuild pipeline (`BuildTailwind` target in `HRM.Client.csproj`), so `dotnet build` / `dotnet run` automatically rebuilds CSS.

---

## 🎬 First-Run Setup

The Setup Wizard appears automatically when no company data exists.

```
┌────────────────────────────────────────────────────┐
│                  Welcome to HRM                    │
│                                                    │
│   ○  Load Demo Data       — 30 employees, payroll  │
│   ○  New Company          — guided wizard          │
│   ○  Reset Database       — clean slate            │
└────────────────────────────────────────────────────┘
```

**Demo mode** is the fastest way to explore every feature. It loads:
- 5 branches · 5 departments · 30 employees
- Payroll records (March 2026)
- Performance evaluations & appraisals
- Leave requests and balances
- Active transfer pipeline

---

## 🔑 Demo Credentials

| Role | Email | Password |
|---|---|---|
| Super Admin | `admin@root.com` | `admin` |
| HR Manager | `hr@root.com` | `hr1234` |
| Employee | `emp@root.com` | `emp123` |

> The Quick Demo Access pills on the login page log you in instantly.

---

## 🗺️ Feature Modules

<details>
<summary><strong>📊 Dashboard</strong></summary>

Real-time HR metrics on the home screen:

- Total staff count · Average FTE · Retention rate
- Recruitment pipeline (Sourced → Interviewed → Offered)
- Employee experience NPS & satisfaction
- Headcount forecast + talent scoring grid
- Active transfers and pending leave request widgets
</details>

<details>
<summary><strong>🏢 Organization</strong></summary>

Full company hierarchy management:

- **Branches** — physical office locations with contact details
- **Departments** — scoped to branches
- **Designations** — job titles with level hierarchy
- **Employees** — full directory, paginated and searchable
</details>

<details>
<summary><strong>🏖️ Leave Management</strong></summary>

Complete leave lifecycle:

- Multiple leave categories (Annual, Sick, Personal, etc.) with per-category yearly limits
- Request → Approve / Reject workflow
- Leave balance tracking per employee per year
</details>

<details>
<summary><strong>💰 Payroll</strong></summary>

Flexible payroll engine:

- **Salary Grades** — pay bands with built-in income and deduction templates
- **Income Components** — allowances, bonuses, any custom additions
- **Deduction Components** — taxes, insurance, loans
- Monthly payroll processing with printable salary slips
- Excel export support
</details>

<details>
<summary><strong>📈 Performance</strong></summary>

360° performance tracking:

- **Evaluations** — scored 0–100 with 5-star rating, linked to evaluator
- **Appraisals** — detailed reviews with goals and achievements
- **Promotions** — designation change pipeline
- **Transfers** — inter-branch / inter-department moves
</details>

<details>
<summary><strong>👤 User Profile</strong></summary>

Self-service profile management:

- Edit personal info, job title, address
- Change password (current password required)
- Upload avatar (JPG/PNG/GIF, max 5 MB) — updates everywhere instantly
- Active session viewer
</details>

<details>
<summary><strong>⚙️ App Settings</strong> (Admin)</summary>

- **Users** — list all accounts with roles
- **Currencies** — multi-currency support
- **Auto Numbers** — configure employee code format, prefix, padding
- **Branding** — company name, tagline, logo upload, favicon upload *(Super Admin only)*
- **Maintenance** — seed demo data, reset database
</details>

<details>
<summary><strong>🖥️ System Settings</strong> (Admin)</summary>

Read-only view of server configuration:

- Identity & password policy
- JWT / cookie settings
- Database provider
- Background jobs
- Email / SMTP
- File storage limits
- Logging levels
</details>

---

## 🎨 White-Label Branding

HRM is designed to be white-labelled. Everything is configurable via **App Settings → Branding** (Super Admin only):

| Setting | Description |
|---|---|
| Company Name | Shown in sidebar, login page, and browser title |
| Tagline | Shown below company name in the sidebar |
| App Title | Shown in the top bar and login subtitle |
| Logo Monogram | 2–4 character badge fallback when no image is set |
| Logo Image | Upload PNG / SVG / JPG (max 2 MB) — sidebar + login icon |
| Favicon | Upload ICO / PNG / SVG — browser tab icon, updated live |

Branding is stored in `HRM.Server/branding.json` and served via a public API endpoint so even the login page reflects your brand before authentication.

### Live preview

The Branding settings page shows a real-time preview of the sidebar, top bar, and login screen as you type.

---

## 🌗 Theming

The theme is controlled by two user-selectable settings, persisted to `localStorage`:

**Dark mode** — toggle via the ☀️ / 🌙 button in the top bar.

**Accent color** — 6 options, each remapping all `indigo-*` Tailwind classes via CSS variables:

| Accent | Color |
|---|---|
| Indigo (default) | `#6366f1` |
| Violet | `#7c3aed` |
| Blue | `#2563eb` |
| Emerald | `#059669` |
| Rose | `#e11d48` |
| Amber | `#d97706` |

---

## 🔌 API Documentation

The API is built with ASP.NET Core Minimal APIs and documented via **Scalar** (OpenAPI).

> Available in development: **https://localhost:7136/scalar/v1**

### Endpoint Groups

| Group | Base Path | Auth |
|---|---|---|
| Auth | `/api/auth` | Public |
| Branding (GET) | `/api/branding` | Public |
| Dashboard | `/api/dashboard` | ✅ Required |
| Employees | `/api/employees` | ✅ Required |
| Branches | `/api/branches` | ✅ Required |
| Departments | `/api/departments` | ✅ Required |
| Designations | `/api/designations` | ✅ Required |
| Leave Categories | `/api/leave-categories` | ✅ Required |
| Leave Requests | `/api/leave-requests` | ✅ Required |
| Payroll | `/api/payroll` | ✅ Required |
| Salary Grades | `/api/salary-grades` | ✅ Required |
| Evaluations | `/api/evaluations` | ✅ Required |
| Profile | `/api/profile` | ✅ Required |
| Currencies | `/api/currencies` | ✅ Required |
| Auto Numbers | `/api/auto-numbers` | ✅ Required |
| System Settings | `/api/system-settings` | ✅ Required |
| System Logs | `/api/system-logs` | ✅ Required |
| Branding (PUT/Upload) | `/api/branding` | ✅ Required |
| Admin / Maintenance | `/api/admin` | ✅ Required |

---

## 🏗️ Architecture

```
Browser (Blazor WASM)
       │  JWT Bearer token
       ▼
ASP.NET Core 10 Server
  ├── Minimal API Endpoints
  ├── ASP.NET Core Identity
  ├── Entity Framework Core 10
  │       └── SQL Server (LocalDB / SQL Server)
  ├── Serilog → File + SQL logs
  └── Static file serving (avatars, branding, Blazor output)
```

**Authentication flow:**
1. Client POSTs credentials to `/api/auth/login`
2. Server returns a signed JWT (24-hour expiry)
3. Client stores token in `localStorage`, attaches it as `Authorization: Bearer …` on every request
4. `JwtAuthStateProvider` exposes auth state to Blazor components

---

## 🛠️ Development

### Tailwind CSS

The project uses a local PostCSS pipeline — **no CDN, no runtime injection**.

```bash
# One-time build
npm run build:css --prefix HRM.Client

# Watch mode during development
npm run watch:css --prefix HRM.Client
```

The MSBuild target `BuildTailwind` runs `build:css` automatically before every `dotnet build`.

### Database migrations

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project HRM.Server --startup-project HRM.Server

# Apply migrations manually (normally auto-applied on startup)
dotnet ef database update --project HRM.Server --startup-project HRM.Server
```

### Logging

Logs are written to:
- **Console** — all levels in development
- **File** — `HRM.Server/Logs/hrm-YYYYMMDD.log` (daily rolling)
- **Database** — `SystemLogs` table (viewable in System Logs page)

---

## ⚙️ Configuration Reference

Key entries in `HRM.Server/appsettings.json`:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HRM;..."
  },
  "Jwt": {
    "Key":          "<replace with a 64+ character random secret>",
    "Issuer":       "HRM.Server",
    "Audience":     "HRM.Client",
    "ExpireMinutes": 1440
  },
  "DefaultAdmin": {
    "Email":    "admin@root.com",
    "Password": "admin"           // ⚠️ change in production
  },
  "Storage": {
    "AvatarPath":         "wwwroot/avatars",
    "MaxFileSize":        5242880,   // 5 MB
    "AllowedExtensions":  ".jpg,.jpeg,.png,.gif"
  },
  "Jobs": {
    "EnableBackgroundJobs": true,
    "PayrollProcessingDay": 25      // auto-process payroll on the 25th
  }
}
```

### Production checklist

- [ ] Replace `Jwt:Key` with a cryptographically random 64-character string
- [ ] Change `DefaultAdmin:Password` immediately after first login
- [ ] Update `ConnectionStrings:DefaultConnection` to a production SQL Server instance
- [ ] Configure `Email` SMTP settings for notifications
- [ ] Set up HTTPS with a valid certificate (not the dev cert)
- [ ] Review `Identity` password policy settings

---

## 📦 Tech Stack

### Backend
| Package | Version | Purpose |
|---|---|---|
| ASP.NET Core | 10.0 | Web framework & Minimal APIs |
| Entity Framework Core | 10.0.5 | ORM |
| ASP.NET Core Identity | 10.0.5 | Auth + user management |
| JwtBearer | 10.0.5 | JWT authentication |
| Serilog | 9.0 | Structured logging |
| ClosedXML | 0.105 | Excel export |
| Scalar | 2.4 | API documentation UI |

### Frontend
| Package | Version | Purpose |
|---|---|---|
| Blazor WebAssembly | 10.0.5 | SPA framework |
| Blazored.LocalStorage | 4.5.0 | Client-side persistence |
| Tailwind CSS | 3.4.19 | Utility-first CSS |
| PostCSS + Autoprefixer | latest | CSS build pipeline |

---

## 📁 Client Pages Reference

| Route | Page | Access |
|---|---|---|
| `/account/login` | Login / Register | Public |
| `/` | Dashboard | All roles |
| `/profile` | User Profile | All roles |
| `/leave` | Leave Management | All roles |
| `/performance` | Performance | All roles |
| `/organization` | Org Structure | HR+ |
| `/payroll` | Payroll | HR+ |
| `/system-logs` | System Logs | Admin+ |
| `/app-settings` | App Settings | Admin+ |
| `/system-settings` | System Settings | Admin+ |
| `/setup` | Setup Wizard | Super Admin |

---

<div align="center">

**Built with ❤️ using .NET 10 · Blazor · Tailwind CSS**

</div>
