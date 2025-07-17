/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Cars.Create
{

    /// <summary>
    /// DTO for the response after creating a car, returning the new car's Id.
    /// </summary>
    public class CreateCarResponseDto
    {
        /// <summary>
        /// Unique identifier of the newly created car.
        /// </summary>
        /// <example>f47ac10b-58cc-4372-a567-0e02b2c3d479</example>
        public Guid CarId { get; }

        public CreateCarResponseDto(Guid carId)
        {
            CarId = carId;
        }
    }
}
