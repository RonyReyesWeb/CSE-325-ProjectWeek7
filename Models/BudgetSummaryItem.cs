namespace PersonalBudgetTracker.Models;

// Read-only view model used by the Dashboard to show spent vs. remaining
// per category for the current month. Never saved to the database.
public class BudgetSummaryItem
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal MonthlyLimit { get; set; }
    public decimal Spent { get; set; }
    public decimal Remaining => MonthlyLimit - Spent;
    public double PercentUsed => MonthlyLimit <= 0 ? 0 : (double)(Spent / MonthlyLimit * 100);
}
