/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Rentals.Dtos;

/// <summary>
/// 🚘 DTO representing an available car result.
/// </summary>
/// <remarks>
/// Used to return information about a car that is available for the given period.
/// </remarks>
public class CarAvailabilityDto
{
    /// <summary>Unique identifier of the car.</summary>
    /// <example>fcd2a7c2-3385-4b39-9e3d-abc123456789</example>
    public Guid Id { get; set; }

    /// <summary>Model of the car.</summary>
    /// <example>Toyota Corolla</example>
    public string Model { get; set; } = string.Empty;

    /// <summary>Type of the car (e.g., Sedan, SUV).</summary>
    /// <example>Sedan</example>
    public string Type { get; set; } = string.Empty;
}