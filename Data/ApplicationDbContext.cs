using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalBudgetTracker.Models;

namespace PersonalBudgetTracker.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Expense>()
            .HasOne(e => e.Category)
            .WithMany(c => c.Expenses)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Budget>()
            .HasOne(b => b.Category)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        // A user can only have one budget per category.
        builder.Entity<Budget>()
            .HasIndex(b => new { b.UserId, b.CategoryId })
            .IsUnique();
    }
}
