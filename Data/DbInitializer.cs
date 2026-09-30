using PersonalBudgetTracker.Models;

namespace PersonalBudgetTracker.Data;

public static class DbInitializer
{
    private static readonly string[] DefaultCategories =
    {
        "Food", "Transportation", "Housing", "Utilities", "Entertainment", "Healthcare", "Other"
    };

    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (!db.Categories.Any())
        {
            foreach (var name in DefaultCategories)
            {
                db.Categories.Add(new Category { Name = name });
            }

            await db.SaveChangesAsync();
        }
    }
}
