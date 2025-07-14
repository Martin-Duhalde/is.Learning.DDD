/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Auth.Dtos;

/// <summary>
/// 🔑 Data Transfer Object (DTO) used for user login.
/// </summary>
/// <remarks>
/// This DTO is sent by the client to log in to the system. It includes the user's credentials: email and password.
/// </remarks>
/// <example>
/// Example request:
/// {
///     "Email": "john.doe@example.com",
///     "Password": "P@ssw0rd123"
/// }
/// </example>
public class LoginDto
{
    /// <summary>
    /// 📧 The email address used to log in.
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    /// <summary>
    /// 🔒 The password associated with the account.
    /// </summary>
    /// <example>P@ssw0rd123</example>
    [Required]
    public string Password { get; set; } = "";
}
