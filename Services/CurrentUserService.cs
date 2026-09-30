using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace PersonalBudgetTracker.Services;

// Small helper so pages don't each repeat the claims lookup for "who is signed in".
public class CurrentUserService
{
    private readonly AuthenticationStateProvider _authProvider;

    public CurrentUserService(AuthenticationStateProvider authProvider)
    {
        _authProvider = authProvider;
    }

    public async Task<string> GetUserIdAsync()
    {
        var state = await _authProvider.GetAuthenticationStateAsync();
        var id = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return id ?? throw new ServiceException("No signed-in user was found. Please log in again.");
    }
}
