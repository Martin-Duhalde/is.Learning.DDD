/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.UseCases.Rentals.Dtos;
using MediatR;

namespace CarRental.UseCases.Rentals.List;

public record GetAllRentalsQuery() : IRequest<List<RentalDto>>;

public class GetAllRentalsQueryHandler : IRequestHandler<GetAllRentalsQuery, List<RentalDto>>
{
    private readonly IRentalRepository _rentalRepo;

    public GetAllRentalsQueryHandler(IRentalRepository rentalRepo)
    {
        _rentalRepo = rentalRepo;
    }

    public async Task<List<RentalDto>> Handle(GetAllRentalsQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepo.ListAllActivesAsync(cancellationToken);

        return rentals.Select(r => new RentalDto
        {
            Id              /**/ = r.Id,
            CustomerId      /**/ = r.CustomerId,
            CarId           /**/ = r.CarId,
            StartDate       /**/ = r.StartDate,
            EndDate         /**/ = r.EndDate

        }).ToList();
    }
}