/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Services.Dtos;

/// <summary>
/// DTO for response after creating a new service.
/// </summary>
public class CreateServiceResponseDto
{
    /// <summary>
    /// Unique ID of the created service.
    /// </summary>
    /// <example>ddda1234-5678-90ab-cdef-1234567890ab</example>
    public Guid ServiceId { get; }

    public CreateServiceResponseDto(Guid id)
    {
        ServiceId = id;
    }
}