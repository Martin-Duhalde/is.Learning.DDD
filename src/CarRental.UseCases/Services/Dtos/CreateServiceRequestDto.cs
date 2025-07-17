
using CarRental.UseCases.Common.Validators;

/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Services.Dtos;

/// <summary>
/// DTO for creating a new service request.
/// </summary>
public class CreateServiceRequestDto
{
    /// <summary>
    /// Date when the service was performed.
    /// </summary>
    /// <example>2025-06-01T08:30:00Z</example>
    [Required(ErrorMessage = "Date is required.")]
    public DateTime Date { get; set; }

    /// <summary>
    /// ID of the car that received the service.
    /// </summary>
    /// <example>e7a1c2d5-4d8a-4c3b-bb99-321aa8c732ea</example>
    [NotEmptyGuid(ErrorMessage = "CarId is required.")]   //[Required(ErrorMessage = "CarId is required.")]
    public Guid CarId { get; set; }
}


