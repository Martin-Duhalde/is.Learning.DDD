/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Services.Dtos;

/// <summary>
/// 🔧 DTO to update an existing service.
/// </summary>
public class UpdateServiceDto
{
    /// <summary>ID of the service.</summary>
    /// <example>ddda1234-5678-90ab-cdef-1234567890ab</example>
    [Required]
    public Guid Id { get; set; }

    /// <summary>Date when the service was performed.</summary>
    /// <example>2025-06-01T08:30:00Z</example>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>ID of the car that received the service.</summary>
    /// <example>e7a1c2d5-4d8a-4c3b-bb99-321aa8c732ea</example>
    [Required]
    public Guid CarId { get; set; }
}