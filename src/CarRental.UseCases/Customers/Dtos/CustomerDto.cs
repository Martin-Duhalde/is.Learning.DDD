/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Customers.Dtos;

/// <summary>
/// 👤 DTO to read customer data.
/// </summary>
public class CustomerDto
{
    /// <summary>Unique ID of the customer.</summary>
    /// <example>bfcd8c7e-fd6b-4d9e-8a34-7eaf9a441dad</example>
    public Guid Id { get; set; }

    /// <summary>Full name of the customer.</summary>
    /// <example>Jane Doe</example>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Address of the customer.</summary>
    /// <example>123 Main Street, New York, NY</example>
    public string Address { get; set; } = string.Empty;
}
