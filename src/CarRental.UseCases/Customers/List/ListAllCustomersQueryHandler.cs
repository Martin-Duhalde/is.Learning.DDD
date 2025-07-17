/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace CarRental.UseCases.Customers.GetAll;

public record ListAllCustomersQuery : IRequest<List<Customer>>;

public class ListAllCustomersQueryHandler : IRequestHandler<ListAllCustomersQuery, List<Customer>>
{
    private readonly CarRentalDbContext _db;

    public ListAllCustomersQueryHandler(CarRentalDbContext db)
    {
        _db = db;
    }

    public async Task<List<Customer>> Handle(ListAllCustomersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Customers.AsNoTracking().ToListAsync(cancellationToken);
    }
}
