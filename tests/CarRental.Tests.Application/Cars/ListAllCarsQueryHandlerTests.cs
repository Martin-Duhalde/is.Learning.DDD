/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Repositories;
using CarRental.Application.Cars.Dtos;
using CarRental.Application.Cars.GetAll;
using CarRental.Domain.Entities;
using CarRental.Tests.Application.TestBuilders;

namespace CarRental.Tests.Application.Cars;

public class ListAllCarsQueryHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly ListAllCarsQueryHandler _handler;

    public ListAllCarsQueryHandlerTests()
    {
        _handler = new ListAllCarsQueryHandler(_carRepo);
    }

    [Fact]
    public async Task should_return_list_of_car_dtos_when_cars_exist()
    {
        var cars = new List<Car>
        {
            DomainBuilder.BuildCar(model: "Model S", type: "Sedan"),
            DomainBuilder.BuildCar(model: "Model 3", type: "Sedan")
        };

        _carRepo.ListAllActivesAsync(Arg.Any<CancellationToken>())
            .Returns(cars);

        var query = new ListAllCarsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Model == "Model S");
        Assert.Contains(result, c => c.Model == "Model 3");
    }

    [Fact]
    public async Task should_return_empty_list_when_no_cars_exist()
    {
        _carRepo.ListAllActivesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Car>());

        var query = new ListAllCarsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}
