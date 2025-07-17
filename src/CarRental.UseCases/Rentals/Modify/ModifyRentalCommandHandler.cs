/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Exceptions;

using MediatR;

namespace CarRental.UseCases.Rentals.Modify;

public record ModifyRentalCommand(Guid RentalId, DateTime NewStartDate, DateTime NewEndDate, Guid? NewCarId) : IRequest<Unit>;


public class ModifyRentalCommandHandler : IRequestHandler<ModifyRentalCommand, Unit>
{
    private readonly IRentalRepository  /**/ _rentalRepository;
    private readonly ICarRepository     /**/ _carRepository;

    public ModifyRentalCommandHandler(IRentalRepository rentalRepository, ICarRepository carRepository)
    {
        _rentalRepository   /**/ = rentalRepository;
        _carRepository      /**/ = carRepository;
    }

    public async Task<Unit> Handle(ModifyRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetActiveByIdAsync(request.RentalId, cancellationToken)
            ?? throw new DomainException("Rental not found.");

        var carId = request.NewCarId ?? rental.CarId;

        if (request.NewEndDate <= request.NewStartDate)
            throw new DomainException("End date must be after start date.");

        var isAvailable = await _carRepository.IsAvailableAsync(carId, request.NewStartDate, request.NewEndDate, cancellationToken);
        if (!isAvailable)
            throw new DomainException("The car is not available for the new selected period.");

        rental.CarId        /**/ = carId;
        rental.StartDate    /**/ = request.NewStartDate;
        rental.EndDate      /**/ = request.NewEndDate;

        await _rentalRepository.UpdateAsync(rental, cancellationToken);

        return Unit.Value;
    }
}