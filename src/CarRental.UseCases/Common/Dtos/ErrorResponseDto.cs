/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Common.Dtos;

/// <summary>
/// 🚨 Data Transfer Object (DTO) used to represent an error response in the API.
/// </summary>
/// <remarks>
/// This DTO is returned whenever an error occurs during an API request, typically in response to validation failures,
/// authorization errors, or internal server issues. It provides details about the error and any additional context
/// that might be helpful for troubleshooting or client-side handling.
/// </remarks>
/// <example>
/// Example error response:
/// {
///     "Error": "Invalid Credentials",
///     "Details": "The username or password provided is incorrect."
/// }
/// </example>
public class ErrorResponseDto
{
    /// <summary>
    /// ⚠️ A brief description of the error.
    /// </summary>
    /// <remarks>
    /// 
    /// This string provides a short error message that typically describes the nature of the issue, such as:
    /// - "Unauthorized": The user is not authorized to perform this action.
    /// - "Bad Request": The request was malformed or missing required information.
    /// - "Internal Server Error": Something went wrong on the server side.
    /// </remarks>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// 📝 Optional detailed information about the error.
    /// </summary>
    /// <remarks>
    /// This property provides additional context about the error to help the client understand why the request failed,
    /// what went wrong, or how to fix it. For example, in the case of invalid credentials, this could explain that
    /// the username or password is incorrect.
    /// </remarks>
    public string? Details { get; set; }
}
