/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;

using MediatR;

namespace CarRental.UseCases.Rentals.Create;

public record CreateRentalCommand(Guid CustomerId, Guid CarId, DateTime StartDate, DateTime EndDate) : IRequest<Guid>;


public class CreateRentalCommandHandler : IRequestHandler<CreateRentalCommand, Guid>
{
    private readonly IRentalRepository  /**/ _rentalRepository;
    private readonly ICarRepository     /**/ _carRepository;

    public CreateRentalCommandHandler(IRentalRepository rentalRepo, ICarRepository carRepo)
    {
        _rentalRepository   /**/ = rentalRepo;
        _carRepository      /**/ = carRepo;
    }

    public async Task<Guid> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        var isAvailable = await _carRepository.IsAvailableAsync(request.CarId, request.StartDate, request.EndDate, cancellationToken);
        if (!isAvailable)
            throw new DomainException("The car is not available for the selected period.");

        var rental = new Rental
        {
            Id              /**/ = Guid.NewGuid(),
            CustomerId      /**/ = request.CustomerId,
            CarId           /**/ = request.CarId,
            StartDate       /**/ = request.StartDate,
            EndDate         /**/ = request.EndDate
        };

        await _rentalRepository.AddAsync(rental, cancellationToken);

        return rental.Id;
    }
}