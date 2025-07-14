/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;

using MediatR;

namespace CarRental.UseCases.Customers.Create;

public class CreateCustomerCommand : IRequest<Guid>
{
    public string FullName { get; set; } = null!;
    public string Address { get; set; } = null!;
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly CarRentalDbContext _db;

    public CreateCustomerCommandHandler(CarRentalDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Address = request.Address
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }
}
