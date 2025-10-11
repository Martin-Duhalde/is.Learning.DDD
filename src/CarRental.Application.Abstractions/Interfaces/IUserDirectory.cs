/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.Application.Abstractions.Interfaces;

/// <summary>
/// Abstraction for querying user information from an external directory (Identity).
/// </summary>
public interface IUserDirectory
{
    Task<UserDirectoryEntry?> GetByIdAsync(string userId, CancellationToken ct = default);
}

/// <summary>
/// Simplified view of a user retrieved from the directory.
/// </summary>
public record UserDirectoryEntry(string? Email);
