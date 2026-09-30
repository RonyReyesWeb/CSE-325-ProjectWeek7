using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTracker.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A category name is required.")]
    [StringLength(50, ErrorMessage = "Category name can't be longer than 50 characters.")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
