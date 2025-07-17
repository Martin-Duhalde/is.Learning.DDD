/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Rentals.Dtos;

/// <summary>
/// 📄 DTO to update an existing rental.
/// </summary>
public class UpdateRentalDto
{
    /// <summary>ID of the rental.</summary>
    /// <example>ddda1234-5678-90ab-cdef-1234567890ab</example>
    [Required]
    public Guid Id { get; set; }

    /// <summary>ID of the customer renting the car.</summary>
    /// <example>bfcd8c7e-fd6b-4d9e-8a34-7eaf9a441dad</example>
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>ID of the car to be rented.</summary>
    /// <example>e7a1c2d5-4d8a-4c3b-bb99-321aa8c732ea</example>
    [Required]
    public Guid CarId { get; set; }

    /// <summary>Start date of the rental.</summary>
    /// <example>2025-07-15T10:00:00Z</example>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>End date of the rental.</summary>
    /// <example>2025-07-20T10:00:00Z</example>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Concurrency version number of the car entity, used for optimistic concurrency control.
    /// Incremented automatically on each update to detect conflicting changes.
    /// </summary>
    /// <example>3</example>
    [Required(ErrorMessage = "Version is required.")]
    public int Version { get; set; }
}