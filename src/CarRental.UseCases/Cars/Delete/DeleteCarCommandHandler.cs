/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Exceptions;

using MediatR;

namespace CarRental.UseCases.Cars.Delete;

public record DeleteCarCommand(Guid Id) : IRequest<Unit>;

public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, Unit>
{
    private readonly ICarRepository _carRepository;

    public DeleteCarCommandHandler(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<Unit> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetActiveByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException($"Car with ID {request.Id} not found.");

        await _carRepository.DeleteAsync(car, cancellationToken);

        return Unit.Value;
    }
}
