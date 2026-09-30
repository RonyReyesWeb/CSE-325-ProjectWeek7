using Microsoft.EntityFrameworkCore;
using PersonalBudgetTracker.Data;
using PersonalBudgetTracker.Models;

namespace PersonalBudgetTracker.Services;

public class ExpenseService : IExpenseService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(ApplicationDbContext db, ILogger<ExpenseService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Expense>> GetAllForUserAsync(string userId)
    {
        try
        {
            return await _db.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load expenses for user {UserId}.", userId);
            throw new ServiceException("We couldn't load your expenses. Please try again.", ex);
        }
    }

    public async Task<Expense?> GetByIdAsync(int id, string userId)
    {
        try
        {
            return await _db.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load expense {Id}.", id);
            throw new ServiceException("We couldn't load that expense.", ex);
        }
    }

    public async Task<Expense> AddAsync(Expense expense)
    {
        try
        {
            _db.Expenses.Add(expense);
            await _db.SaveChangesAsync();
            return expense;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add expense for user {UserId}.", expense.UserId);
            throw new ServiceException("We couldn't save that expense. Please try again.", ex);
        }
    }

    public async Task UpdateAsync(Expense expense)
    {
        try
        {
            var existing = await _db.Expenses
                .FirstOrDefaultAsync(e => e.Id == expense.Id && e.UserId == expense.UserId);

            if (existing is null)
                throw new ServiceException("That expense no longer exists.");

            existing.Description = expense.Description;
            existing.Amount = expense.Amount;
            existing.Date = expense.Date;
            existing.CategoryId = expense.CategoryId;

            await _db.SaveChangesAsync();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update expense {Id}.", expense.Id);
            throw new ServiceException("We couldn't update that expense. Please try again.", ex);
        }
    }

    public async Task DeleteAsync(int id, string userId)
    {
        try
        {
            var expense = await _db.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (expense is null) return;

            _db.Expenses.Remove(expense);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete expense {Id}.", id);
            throw new ServiceException("We couldn't delete that expense. Please try again.", ex);
        }
    }
}
