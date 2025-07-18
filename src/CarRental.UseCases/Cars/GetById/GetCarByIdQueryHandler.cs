/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Databases;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace CarRental.UseCases.Cars.GetById;

public record GetCarByIdQuery(Guid Id) : IRequest<Car>;

public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, Car>
{
    private readonly CarRentalDbContext _db;

    public GetCarByIdQueryHandler(CarRentalDbContext db)
    {
        _db = db;
    }

    public async Task<Car> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
    {
        var car = await _db.Cars.FirstOrDefaultAsync(c => c.IsActive && c.Id == request.Id, cancellationToken);
        if (car == null)
            throw new DomainNotFoundException(typeof(Car).Name, request.Id);

        return car;
    }
}
