/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Customers.Dtos;

/// <summary>
/// 👤 DTO to create a new customer.
/// </summary>
/// <remarks>
/// Includes full name and address.
/// </remarks>
public class CreateCustomerDto
{
    /// <summary>Full name of the customer.</summary>
    /// <example>Jane Doe</example>
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Address of the customer.</summary>
    /// <example>123 Main Street, New York, NY</example>
    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;
}
