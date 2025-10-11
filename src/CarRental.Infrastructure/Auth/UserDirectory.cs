/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Interfaces;

using Microsoft.AspNetCore.Identity;

namespace CarRental.Infrastructure.Auth;

/// <summary>
/// Adapter that bridges Identity's UserManager with the application layer.
/// </summary>
public class UserDirectory : IUserDirectory
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserDirectory(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDirectoryEntry?> GetByIdAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        return new UserDirectoryEntry(user.Email);
    }
}
