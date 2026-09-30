using System.ComponentModel.DataAnnotations;

namespace PersonalBudgetTracker.Models;

public class Expense
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please add a short description.")]
    [StringLength(100)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter an amount.")]
    [Range(0.01, 1_000_000, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Please choose a category.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a category.")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Owning user. Set by the service layer from the signed-in user — never
    // exposed to or editable through the UI directly.
    public string UserId { get; set; } = string.Empty;
}
