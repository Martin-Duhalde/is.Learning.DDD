/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Rentals.Dtos;

/// <summary>
/// 📈 DTO representing rental statistics by car type.
/// </summary>
public class RentalStatDto
{
    /// <summary>Type of the car.</summary>
    /// <example>SUV</example>
    public string CarType { get; set; } = string.Empty;

    /// <summary>Number of rentals for this type.</summary>
    /// <example>12</example>
    public int Count { get; set; }

    /// <summary>Usage percentage of this type.</summary>
    /// <example>34.5</example>
    public double Percentage { get; set; }
}