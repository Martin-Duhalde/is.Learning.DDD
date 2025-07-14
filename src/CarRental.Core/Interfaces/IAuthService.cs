/// MIT License © 2025 Martín Duhalde + ChatGPT
///
/// Interface: IAuthService
/// 📦 Core: Defines the contract for user authentication services.
///
namespace CarRental.Core.Interfaces;

/// <summary>
/// 🔐 Authentication service interface.
///
/// Provides the essential methods for user registration and login.
/// Implementations should handle credential validation, token generation,
/// and appropriate exception handling.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user into the system.
    /// </summary>
    /// <param name="fullName">The user's full name.</param>
    /// <param name="email">The user's email address, used as a unique identifier.</param>
    /// <param name="password">The user's plain-text password (must be hashed internally).</param>
    /// <returns>
    /// A <see cref="string"/> representing the authentication token generated after successful registration.
    /// </returns>
    Task<string> RegisterAsync(string fullName, string email, string password);

    /// <summary>
    /// Authenticates an existing user based on email and password.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The plain-text password to be verified.</param>
    /// <returns>
    /// A <see cref="string"/> representing the authentication token if credentials are valid.
    /// </returns>
    Task<string> LoginAsync(string email, string password);
}
