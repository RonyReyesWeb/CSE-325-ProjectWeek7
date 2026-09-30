using Microsoft.EntityFrameworkCore;
using PersonalBudgetTracker.Data;
using PersonalBudgetTracker.Models;

namespace PersonalBudgetTracker.Services;

public class BudgetService : IBudgetService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(ApplicationDbContext db, ILogger<BudgetService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Budget>> GetAllForUserAsync(string userId)
    {
        try
        {
            return await _db.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.Category!.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load budgets for user {UserId}.", userId);
            throw new ServiceException("We couldn't load your budgets. Please try again.", ex);
        }
    }

    public async Task<Budget?> GetByIdAsync(int id, string userId)
    {
        try
        {
            return await _db.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load budget {Id}.", id);
            throw new ServiceException("We couldn't load that budget.", ex);
        }
    }

    public async Task<Budget> AddAsync(Budget budget)
    {
        try
        {
            var alreadyExists = await _db.Budgets
                .AnyAsync(b => b.UserId == budget.UserId && b.CategoryId == budget.CategoryId);
            if (alreadyExists)
                throw new ServiceException("You already have a budget for that category. Edit it instead of adding a new one.");

            _db.Budgets.Add(budget);
            await _db.SaveChangesAsync();
            return budget;
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add budget for user {UserId}.", budget.UserId);
            throw new ServiceException("We couldn't save that budget. Please try again.", ex);
        }
    }

    public async Task UpdateAsync(Budget budget)
    {
        try
        {
            var existing = await _db.Budgets
                .FirstOrDefaultAsync(b => b.Id == budget.Id && b.UserId == budget.UserId);
            if (existing is null)
                throw new ServiceException("That budget no longer exists.");

            existing.MonthlyLimit = budget.MonthlyLimit;
            await _db.SaveChangesAsync();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update budget {Id}.", budget.Id);
            throw new ServiceException("We couldn't update that budget. Please try again.", ex);
        }
    }

    public async Task DeleteAsync(int id, string userId)
    {
        try
        {
            var budget = await _db.Budgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (budget is null) return;

            _db.Budgets.Remove(budget);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete budget {Id}.", id);
            throw new ServiceException("We couldn't delete that budget. Please try again.", ex);
        }
    }

    public async Task<List<BudgetSummaryItem>> GetSummaryForUserAsync(string userId, int year, int month)
    {
        try
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var budgets = await _db.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            var expenses = await _db.Expenses
                .Where(e => e.UserId == userId && e.Date >= monthStart && e.Date < monthEnd)
                .ToListAsync();

            return budgets
                .Select(b => new BudgetSummaryItem
                {
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category?.Name ?? "Unknown",
                    MonthlyLimit = b.MonthlyLimit,
                    Spent = expenses.Where(e => e.CategoryId == b.CategoryId).Sum(e => e.Amount)
                })
                .OrderByDescending(s => s.PercentUsed)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build budget summary for user {UserId}.", userId);
            throw new ServiceException("We couldn't load your budget summary. Please try again.", ex);
        }
    }
}
