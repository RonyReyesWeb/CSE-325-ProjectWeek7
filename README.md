# Personal Budget & Expense Tracker

A .NET 8 Blazor Server application for tracking personal expenses against
monthly budgets, built for CSE 325 (BYU-Idaho). Built by Rony Reyes and
Chukwunalu Opute.

## Features

- **User authentication** — register/login with ASP.NET Core Identity; all
  expense and budget data is scoped to the signed-in user.
- **CRUD** for Categories, Expenses, and Budgets.
- **Dashboard** — a budget-summary view showing, per category, the monthly
  limit, amount spent so far this month, remaining balance, and a progress
  bar (turns red if a category goes over budget).
- **Error handling** — the service layer never lets raw database/EF
  exceptions reach the UI. Every service method catches exceptions, logs the
  real error, and raises a `ServiceException` with a plain-English message
  that the Razor components display in a dismissible alert banner.
  Form-level validation (required fields, amount ranges, etc.) uses data
  annotations and Blazor's `EditForm`/`DataAnnotationsValidator`.

## Tech stack

- ASP.NET Core 8 / Blazor Server
- ASP.NET Core Identity (authentication)
- Entity Framework Core 8 with SQLite
- Bootstrap 5 (via CDN) for styling

## Project structure

```
Data/         ApplicationDbContext, ApplicationUser (Identity), DB seeding
Models/       Category, Expense, Budget, BudgetSummaryItem (view model)
Services/     Interfaces + implementations for all CRUD and business logic
Pages/        Blazor pages (Dashboard, Expenses, Budgets, Categories, Home)
Pages/Account/  Razor Pages for Login/Register/Logout (kept as Razor Pages,
              not Blazor components, because signing in writes an auth
              cookie — that has to happen on a normal HTTP request/response,
              not inside a live Blazor Server circuit)
Shared/       MainLayout, NavMenu, RedirectToLogin
wwwroot/      Static assets (site.css)
```

## Running it locally

1. **Prerequisites**: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   and the EF Core CLI tool:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
2. **Restore packages**:
   ```bash
   dotnet restore
   ```
3. **Create the initial migration** (this repo ships without a `Migrations`
   folder — you only need to do this once):
   ```bash
   dotnet ef migrations add InitialCreate
   ```
4. **Run the app**. `Program.cs` calls `db.Database.Migrate()` on startup, so
   the SQLite database file and tables are created automatically the first
   time you run it:
   ```bash
   dotnet run
   ```
5. Open the URL shown in the console (something like `https://localhost:5001`),
   register a new account, and start adding categories, budgets, and expenses.

## Deploying

Any host that supports ASP.NET Core 8 and WebSockets (required for Blazor
Server's SignalR connection) works — e.g. **Render** or **Azure App Service**.
General steps:

1. Push this repo to GitHub.
2. Create a new Web Service (Render) or App Service (Azure) pointing at the
   repo, with the .NET 8 runtime.
3. Set the `ConnectionStrings__DefaultConnection` environment variable if you
   want the database file stored somewhere other than the default
   `budgettracker.db` in the app's working directory. (Note: SQLite's file
   is not persistent across deploys/restarts on most free hosting tiers —
   fine for a class demo, but for anything long-lived consider swapping to a
   managed Postgres/SQL Server database later.)
4. Make sure WebSockets are enabled on the host (on by default on Render; on
   Azure App Service, turn on "Web sockets" under Configuration > General
   settings).
