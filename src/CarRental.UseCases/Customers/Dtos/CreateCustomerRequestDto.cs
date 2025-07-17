/// MIT License © 2025 Martín Duhalde + ChatGPT

using System.ComponentModel.DataAnnotations;

namespace CarRental.UseCases.Customers.Create
{
    /// <summary>
    /// 👤 DTO for creating a new customer request.
    /// </summary>
    public class CreateCustomerRequestDto
    {
        /// <summary>Full name of the customer.</summary>
        /// <example>Jane Doe</example>
        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(100, ErrorMessage = "FullName length can't exceed 100 characters.")]
        public string FullName    /**/ { get; set; } = string.Empty;

        /// <summary>Address of the customer.</summary>
        /// <example>123 Main Street, New York, NY</example>
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address length can't exceed 200 characters.")]
        public string Address     /**/ { get; set; } = string.Empty;
    }
}
