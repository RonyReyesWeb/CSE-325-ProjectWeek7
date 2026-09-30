using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTracker.Models;

public class Budget
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please choose a category.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a category.")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [Required(ErrorMessage = "Please enter a monthly limit.")]
    [Range(0, 1_000_000, ErrorMessage = "Limit must be 0 or greater.")]
    public decimal MonthlyLimit { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
}
