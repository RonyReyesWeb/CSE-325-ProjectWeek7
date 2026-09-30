using PersonalBudgetTracker.Models;

namespace PersonalBudgetTracker.Services;

public interface IExpenseService
{
    Task<List<Expense>> GetAllForUserAsync(string userId);
    Task<Expense?> GetByIdAsync(int id, string userId);
    Task<Expense> AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(int id, string userId);
}
