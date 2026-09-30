namespace PersonalBudgetTracker.Services;

// Thrown by the service layer whenever an operation fails in a way the UI
// should show to the user (validation, not-found, db conflicts, etc.), so
// every Razor component only needs one catch block instead of guessing at
// raw exception types (and raw DB/EF errors never leak to the screen).
public class ServiceException : Exception
{
    public ServiceException(string message) : base(message) { }
    public ServiceException(string message, Exception inner) : base(message, inner) { }
}
