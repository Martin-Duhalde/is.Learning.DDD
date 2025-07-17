/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Cars.Create
{
    /// <summary>
    /// DTO for creating a new car request.
    /// </summary>
    public class CreateCarRequestDto
    {
        /// <summary>
        /// Model of the car.
        /// </summary>
        /// <example>Toyota Corolla</example>
        [Required(ErrorMessage = "Model is required.")]
        [StringLength(100, ErrorMessage = "Model length can't exceed 100 characters.")]
        public string Model  /**/ { get; set; } = string.Empty;

        /// <summary>
        /// Type of the car (e.g., Sedan, SUV).
        /// </summary>
        /// <example>SUV</example>
        [Required(ErrorMessage = "Type is required.")]
        [StringLength(50, ErrorMessage = "Type length can't exceed 50 characters.")]
        public string Type   /**/ { get; set; } = string.Empty;
    }
}
