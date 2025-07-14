/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Statistics.Dtos;

/// <summary>
/// 📈 DTO representing a ranked car type usage.
/// </summary>
public class TopCarTypeDto
{
    /// <summary>Type of the car (e.g., SUV, Sedan)</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Number of times it was rented</summary>
    public int Count { get; set; }

    /// <summary>Percentage of total rentals</summary>
    public double Percentage { get; set; }
}
