/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Infrastructure.Databases;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace CarRental.UseCases.Customers.Update;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly CarRentalDbContext _db;

    public UpdateCustomerCommandHandler(CarRentalDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (customer == null)
            throw new KeyNotFoundException("Customer not found");

        customer.FullName = request.FullName;
        customer.Address = request.Address;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    Task IRequestHandler<UpdateCustomerCommand>.Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        return Handle(request, cancellationToken);
    }
}
