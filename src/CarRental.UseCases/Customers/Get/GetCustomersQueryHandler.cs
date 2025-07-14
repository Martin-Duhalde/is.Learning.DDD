/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Infrastructure.Databases;
using CarRental.UseCases.Customers.Dtos;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace CarRental.UseCases.Customers.Get;

public class GetCustomersQuery : IRequest<List<CustomerDto>> { }

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
{
    private readonly CarRentalDbContext _db;

    public GetCustomersQueryHandler(CarRentalDbContext db)
    {
        _db = db;
    }

    public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Customers
            .Select(c => new CustomerDto
            {
                Id          /**/ = c.Id,
                FullName    /**/ = c.FullName,
                Address     /**/ = c.Address
            })
            .ToListAsync(cancellationToken);
    }
}
