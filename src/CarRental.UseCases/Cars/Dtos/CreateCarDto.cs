/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Cars.Dtos;

/// <summary>
/// 🚗 DTO to create a new car.
/// </summary>
/// <remarks>
/// Used to send data necessary to create a car in the system.
/// </remarks>
public class CreateCarDto
{
    /// <summary>Model of the car.</summary>
    /// <example>Toyota Corolla</example>
    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    /// <summary>Type of the car (e.g., Sedan, SUV).</summary>
    /// <example>Sedan</example>
    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;
}