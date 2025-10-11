/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;

using MediatR;

namespace CarRental.Application.Cars.GetById;

public record GetCarByIdQuery(Guid Id) : IRequest<Car>;

public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, Car>
{
    private readonly ICarRepository _carRepository;

    public GetCarByIdQueryHandler(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<Car> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetActiveByIdAsync(request.Id, cancellationToken);
        if (car == null)
            throw new DomainNotFoundException(typeof(Car).Name, request.Id);

        return car;
    }
}
