/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Auth.Dtos;

/// <summary>
/// 📝 Data Transfer Object (DTO) used for user registration.
/// </summary>
/// <remarks>
/// This DTO is used when a new user registers into the system. It collects necessary credentials and profile information
/// such as email, password, and full name.
/// </remarks>
/// <example>
/// Example request:
/// {
///     "Email": "john.doe@example.com",
///     "Password": "P@ssw0rd123",
///     "FullName": "John Doe"
/// }
/// </example>
public class RegisterDto
{
    /// <summary>
    /// 📧 The email address of the new user.
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = "";

    /// <summary>
    /// 🔒 The password chosen by the user.
    /// </summary>
    /// <example>P@ssw0rd123</example>
    public string Password { get; set; } = "";

    /// <summary>
    /// 🙍‍♂️ Full name of the user.
    /// </summary>
    /// <example>John Doe</example>
    public string FullName { get; set; } = "";
}
