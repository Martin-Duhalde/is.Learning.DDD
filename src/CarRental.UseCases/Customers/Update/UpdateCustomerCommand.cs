/// MIT License © 2025 Martín Duhalde + ChatGPT

using MediatR;

namespace CarRental.UseCases.Customers.Update;

public class UpdateCustomerCommand : IRequest
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Address { get; set; } = null!;
}
