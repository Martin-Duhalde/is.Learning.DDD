/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Auth.Dtos;

/// <summary>
/// 🔐 Data Transfer Object (DTO) representing the result of a successful login.
/// </summary>
/// <remarks>
/// This DTO is returned after a user successfully authenticates. It contains the JWT token,
/// user identifier, and email address to be used in the client for further authenticated requests.
/// </remarks>
/// <example>
/// Example response:
/// {
///     "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
///     "UserId": "12345",
///     "Email": "john.doe@example.com"
/// }
/// </example>
public class LoginResultDto
{
    /// <summary>
    /// 🪙 JSON Web Token (JWT) used for authenticating further requests.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string Token { get; set; } = "";

    /// <summary>
    /// 🆔 Unique identifier of the logged-in user.
    /// </summary>
    /// <example>12345</example>
    public string UserId { get; set; } = "";

    /// <summary>
    /// 📧 The email address of the logged-in user.
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = "";
}
