using Microsoft.AspNetCore.Identity;

namespace PersonalBudgetTracker.Data;

// Extends the default Identity user. Kept minimal for now — add profile
// fields here later (e.g. DisplayName, currency preference) if needed.
public class ApplicationUser : IdentityUser
{
}
