/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Rentals.Dtos;

/// <summary>
/// 🛠️ DTO representing a scheduled car service.
/// </summary>
/// <remarks>
/// Used to display upcoming services for cars in a given date range.
/// </remarks>
public class UpcomingServiceDto
{
    /// <summary>Model of the car.</summary>
    /// <example>Toyota Corolla</example>
    public string Model { get; set; } = string.Empty;

    /// <summary>Type of the car.</summary>
    /// <example>SUV</example>
    public string Type { get; set; } = string.Empty;

    /// <summary>Date of the scheduled service.</summary>
    /// <example>2025-08-14</example>
    public DateTime Date { get; set; }
}