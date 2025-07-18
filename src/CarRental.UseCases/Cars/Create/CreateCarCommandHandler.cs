/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;

using MediatR;

namespace CarRental.UseCases.Cars.Create;

public record CreateCarCommand(string Model, string Type) : IRequest<Guid>;

public class CreateCarCommandHandler : IRequestHandler<CreateCarCommand, Guid>
{
    private readonly ICarRepository _carRepository;

    public CreateCarCommandHandler(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<Guid> Handle(CreateCarCommand request, CancellationToken cancellationToken)
    {
        /// Check if a car with the same Model and Type exists (excluding Deleted)
        var existingCars = await _carRepository.FindByModelAndTypeAsync(request.Model, request.Type, cancellationToken);

        /// Domain Helth Check
        if (existingCars.Count > 1) /**/ throw new DomainException("Data inconsistency: multiple cars with the same model and type exist.");
        
        /// If any active car  already exists with same model and type, block creation
        if (existingCars.Any())     /**/ throw new DomainException("A car with the same model and type already exists.");

        var car = new Car
        {
            Id          /**/ = Guid.NewGuid(),
            Model       /**/ = request.Model,
            Type        /**/ = request.Type,
            Services    /**/ = new List<Service>(),
            IsActive    /**/ = true,                /// Default status Active on create
            Version     /**/ = 1,                   /// Version initialized at 1
        };

        await _carRepository.AddAsync(car, cancellationToken);
        
        return car.Id;
    }
}
