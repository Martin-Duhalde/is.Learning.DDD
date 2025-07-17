/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Databases;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace CarRental.UseCases.Customers.Update;

public record UpdateCustomerCommand(Guid Id, string FullName, string Address) : IRequest<Unit>;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
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
            throw new DomainException("Customer not found");

        customer.FullName   /**/ = request.FullName;
        customer.Address    /**/ = request.Address;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
