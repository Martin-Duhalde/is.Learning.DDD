/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Exceptions;

using MediatR;

namespace CarRental.UseCases.Cars.Update;

public record UpdateCarCommand(Guid Id, string Model, string Type, int Version) :  IRequest<Unit>;

public class UpdateCarCommandHandler : IRequestHandler<UpdateCarCommand, Unit>
{
    private readonly ICarRepository _carRepository;

    public UpdateCarCommandHandler(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<Unit> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetActiveByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Car not found.");

        if (request.Version != car.Version)
            throw new ConcurrencyConflictException("The car has been modified by another user or process.");
        
        car.Model   /**/ = request.Model;
        car.Type    /**/ = request.Type;

        await _carRepository.UpdateAsync(car, cancellationToken);

        return Unit.Value;
    }
}
