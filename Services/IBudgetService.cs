using PersonalBudgetTracker.Models;

namespace PersonalBudgetTracker.Services;

public interface IBudgetService
{
    Task<List<Budget>> GetAllForUserAsync(string userId);
    Task<Budget?> GetByIdAsync(int id, string userId);
    Task<Budget> AddAsync(Budget budget);
    Task UpdateAsync(Budget budget);
    Task DeleteAsync(int id, string userId);
    Task<List<BudgetSummaryItem>> GetSummaryForUserAsync(string userId, int year, int month);
}
