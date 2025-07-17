/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Cars.Dtos;

/// <summary>
/// 🚗 DTO to update an existing car.
/// </summary>
/// <remarks>
/// Includes the ID to identify the car to update.
/// </remarks>
public class UpdateCarDto
{
    /// <summary>Unique ID of the car.</summary>
    /// <example>e7a1c2d5-4d8a-4c3b-bb99-321aa8c732ea</example>
    [Required]
    public Guid Id { get; set; }

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

    /// <summary>
    /// Concurrency version number of the car entity, used for optimistic concurrency control.
    /// Incremented automatically on each update to detect conflicting changes.
    /// </summary>
    /// <example>3</example>
    [Required(ErrorMessage = "Version is required.")]
    public int Version { get; set; }
}