/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Rentals.Dtos;

using MediatR;

namespace CarRental.UseCases.Rentals.Get;
public record GetRentalByIdQuery(Guid RentalId) : IRequest<RentalDto>;

public class GetRentalByIdQueryHandler : IRequestHandler<GetRentalByIdQuery, RentalDto>
{
    private readonly IRentalRepository _rentalRepo;

    public GetRentalByIdQueryHandler(IRentalRepository rentalRepo)
    {
        _rentalRepo = rentalRepo;
    }

    public async Task<RentalDto> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepo.GetActiveByIdAsync(request.RentalId, cancellationToken)
            ?? throw new DomainException("Rental not found.");

        return new RentalDto
        {
            Id          /**/ = rental.Id,
            CustomerId  /**/ = rental.CustomerId,
            CarId       /**/ = rental.CarId,
            StartDate   /**/ = rental.StartDate,
            EndDate     /**/ = rental.EndDate
        };
    }
}