/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.UseCases.Cars.GetAll;

namespace CarRental.Tests.UseCases.Cars;

public class ListAllCarsQueryHandlerTests
{
    private readonly ICarRepository              /**/ _carRepo = Substitute.For<ICarRepository>();
    private readonly ListAllCarsQueryHandler     /**/ _handler;

    public ListAllCarsQueryHandlerTests()
    {
        _handler = new ListAllCarsQueryHandler(_carRepo);
    }

    [Fact]
    public async Task should_return_all_active_cars_as_dto()
    {
        // Arrange
        var cars = new List<Car>
        {
            new() { Id = Guid.NewGuid(), Model = "Toyota Corolla", Type = "Sedan", IsActive = true },
            new() { Id = Guid.NewGuid(), Model = "Ford Focus", Type = "Hatchback", IsActive = true }
        };

        _carRepo.ListAllActivesAsync(Arg.Any<CancellationToken>())
                .Returns(cars);

        var query = new ListAllCarsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cars.Count, result.Count);

        foreach (var car in cars)
        {
            Assert.Contains(result, dto =>
                dto.Id == car.Id &&
                dto.Model == car.Model &&
                dto.Type == car.Type
            );
        }
    }

    [Fact]
    public async Task should_return_empty_list_when_no_active_cars()
    {
        // Arrange
        _carRepo.ListAllActivesAsync(Arg.Any<CancellationToken>())
                .Returns(new List<Car>());

        var query = new ListAllCarsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
