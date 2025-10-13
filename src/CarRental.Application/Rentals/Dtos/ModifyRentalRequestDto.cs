/// MIT License © 2025 Martín Duhalde + ChatGPT

using System.ComponentModel.DataAnnotations;

namespace CarRental.Application.Rentals.Dtos;

/// <summary>
/// 🔄 DTO usado por la API para modificar una reserva existente.
/// </summary>
public class ModifyRentalRequestDto
{
    /// <summary>Fecha de inicio solicitada para la reserva.</summary>
    [Required]
    public DateTime NewStartDate { get; set; }

    /// <summary>Fecha de fin solicitada para la reserva.</summary>
    [Required]
    public DateTime NewEndDate { get; set; }

    /// <summary>ID del auto a asignar. Si es <c>null</c>, se mantiene el auto actual.</summary>
    public Guid? NewCarId { get; set; }
}
