using Microsoft.EntityFrameworkCore;
using PersonalBudgetTracker.Data;
using PersonalBudgetTracker.Models;

namespace PersonalBudgetTracker.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ApplicationDbContext db, ILogger<CategoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        try
        {
            return await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories.");
            throw new ServiceException("We couldn't load your categories. Please try again.", ex);
        }
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        try
        {
            return await _db.Categories.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load category {Id}.", id);
            throw new ServiceException("We couldn't load that category.", ex);
        }
    }

    public async Task<Category> AddAsync(Category category)
    {
        try
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add category {Name}.", category.Name);
            throw new ServiceException("We couldn't save that category. Please try again.", ex);
        }
    }

    public async Task UpdateAsync(Category category)
    {
        try
        {
            var existing = await _db.Categories.FindAsync(category.Id);
            if (existing is null)
                throw new ServiceException("That category no longer exists.");

            existing.Name = category.Name;
            await _db.SaveChangesAsync();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update category {Id}.", category.Id);
            throw new ServiceException("We couldn't update that category. Please try again.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var category = await _db.Categories.FindAsync(id);
            if (category is null) return;

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Most likely a foreign-key conflict (expenses/budgets still use this category).
            _logger.LogWarning(ex, "Could not delete category {Id} - likely still in use.", id);
            throw new ServiceException("This category is still used by some expenses or budgets, so it can't be deleted.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete category {Id}.", id);
            throw new ServiceException("We couldn't delete that category. Please try again.", ex);
        }
    }
}
