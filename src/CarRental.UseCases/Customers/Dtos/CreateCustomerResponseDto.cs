/// MIT License © 2025 Martín Duhalde + ChatGPT

namespace CarRental.UseCases.Customers.Create
{
    /// <summary>
    /// 👤 DTO for the response after creating a customer, returning the new customer's Id.
    /// </summary>
    public class CreateCustomerResponseDto
    {
        /// <summary>Identifier of the newly created customer.</summary>
        public Guid CustomerId    /**/ { get; }

        public CreateCustomerResponseDto(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
