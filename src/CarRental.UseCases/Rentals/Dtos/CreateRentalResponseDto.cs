/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Rentals.Dtos;
public class CreateRentalResponseDto
{
    public Guid RentalId { get; set; }

    public CreateRentalResponseDto(Guid rentalId)
    {
        RentalId = rentalId;
    }
}