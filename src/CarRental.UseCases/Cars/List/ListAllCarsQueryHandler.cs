/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.UseCases.Cars.Dtos;

using MediatR;

namespace CarRental.UseCases.Cars.GetAll;

public record ListAllCarsQuery() : IRequest<List<CarDto>>;

public class ListAllCarsQueryHandler : IRequestHandler<ListAllCarsQuery, List<CarDto>>
{
    private readonly ICarRepository _carRepo;

    public ListAllCarsQueryHandler(ICarRepository carRepo)
    {
        _carRepo = carRepo;
    }

    public async Task<List<CarDto>> Handle(ListAllCarsQuery request, CancellationToken cancellationToken)
    {
        var cars = await _carRepo.ListAllActivesAsync(cancellationToken);

        return cars.Select(c => new CarDto
        {
            Id              /**/ = c.Id,
            Model           /**/ = c.Model,
            Type            /**/ = c.Type,
        }).ToList();
    }
}
