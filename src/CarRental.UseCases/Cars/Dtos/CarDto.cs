/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Cars.Dtos;

/// <summary>
/// 🚗 DTO to read car data.
/// </summary>
public class CarDto
{
    /// <summary>Unique ID of the car.</summary>
    /// <example>e7a1c2d5-4d8a-4c3b-bb99-321aa8c732ea</example>
    public Guid Id { get; set; }

    /// <summary>Model of the car.</summary>
    /// <example>Toyota Corolla</example>
    public string Model { get; set; } = string.Empty;

    /// <summary>Type of the car (e.g., Sedan, SUV).</summary>
    /// <example>Sedan</example>
    public string Type { get; set; } = string.Empty;

    // <summary>Concurrency version of the car entity.</summary>
    /// <example>3</example>
    public int Version { get; set; }
}