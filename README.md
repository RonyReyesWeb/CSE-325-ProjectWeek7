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

## Deploying (GitHub + Render)

GitHub Pages only hosts static sites, so it can't run a Blazor Server app.
The code lives on GitHub, and [Render](https://render.com) runs it for free
using the `Dockerfile` in this repo.

Files used for deployment:

| File | Purpose |
|------|---------|
| `Dockerfile` | Builds and runs the app in a .NET 8 container |
| `.dockerignore` | Keeps `bin/`, `obj/` and local `.db` files out of the image |
| `render.yaml` | Render Blueprint: creates the web service automatically |
| `.github/workflows/build.yml` | GitHub Actions: builds the app on every push |

Steps:

1. Commit and push to GitHub:
   ```bash
   git add .
   git commit -m "Add deployment setup"
   git push
   ```
2. Check the **Actions** tab on GitHub — the *Build* workflow should be green.
3. Sign in to [render.com](https://render.com) with your GitHub account.
4. Click **New → Blueprint**, pick this repository, and click **Apply**.
   Render reads `render.yaml` and builds the Docker image (first build takes
   a few minutes).
5. Open the `https://personal-budget-tracker-xxxx.onrender.com` URL Render
   gives you, register an account, and use the app.

After that, every push to `main` redeploys automatically.

Notes:

- The free tier sleeps after ~15 minutes without traffic; the first visit
  after that takes ~30–60 seconds to wake up.
- The SQLite database is stored inside the container, so **data is reset on
  every redeploy/restart**. That is fine for a class demo; for permanent data,
  add a Render persistent disk mounted at `/app/data` (paid plan) or switch to
  a managed database.
