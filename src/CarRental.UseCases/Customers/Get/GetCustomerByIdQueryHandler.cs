/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Infrastructure.Databases;
using CarRental.UseCases.Customers.Dtos;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace CarRental.UseCases.Customers.Get;

public class GetCustomerByIdQuery : IRequest<CustomerDto>
{
    public Guid Id { get; set; }
}

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly CarRentalDbContext _db;

    public GetCustomerByIdQueryHandler(CarRentalDbContext db)
    {
        _db = db;
    }

    public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (customer == null)
            throw new KeyNotFoundException("Customer not found");

        return new CustomerDto
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Address = customer.Address
        };
    }
}
