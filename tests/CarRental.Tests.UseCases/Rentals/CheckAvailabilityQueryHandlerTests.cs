/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.UseCases.Rentals.CheckAvailability;

namespace CarRental.Tests.UseCases.Rentals;

public class CheckAvailabilityQueryHandlerTests
{
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly CheckAvailabilityQueryHandler _handler;

    public CheckAvailabilityQueryHandlerTests()
    {
        _handler = new CheckAvailabilityQueryHandler(_carRepo);
    }

    [Fact]
    public async Task should_return_available_cars_filtered_by_type_and_model()
    {
        // Arrange
        var start   /**/ = DateTime.UtcNow.AddDays(1);
        var end     /**/ = DateTime.UtcNow.AddDays(3);

        var car1 = new Car { Id = Guid.NewGuid(), Model = "ModelX", Type = "SUV" };
        var car2 = new Car { Id = Guid.NewGuid(), Model = "ModelX", Type = "SUV" };
        var car3 = new Car { Id = Guid.NewGuid(), Model = "Another", Type = "Sedan" };

        _carRepo.ListAllActivesAsync(Arg.Any<CancellationToken>())
                .Returns([car1, car2, car3]);

        _carRepo.IsAvailableAsync(car1.Id, start, end, Arg.Any<CancellationToken>())
                .Returns(true);

        _carRepo.IsAvailableAsync(car2.Id, start, end, Arg.Any<CancellationToken>())
                .Returns(false);

        var query = new CheckAvailabilityQuery(start, end, "SUV", "ModelX");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);

        Assert.Equal(car1.Id    /**/ , result[0].Id);
        Assert.Equal("SUV"      /**/ , result[0].Type);
        Assert.Equal("ModelX"   /**/ , result[0].Model);
    }

    [Fact]
    public async Task should_return_empty_list_if_no_cars_match_filters()
    {
        // Arrange
        var car = new Car { Id = Guid.NewGuid(), Model = "Wrong", Type = "Truck" };

        _carRepo.ListAllActivesAsync(Arg.Any<CancellationToken>())
                .Returns([car]);

        var query = new CheckAvailabilityQuery(
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2),
            "SUV", "ModelX");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task should_return_empty_list_if_no_car_is_available()
    {
        // Arrange
        var car = new Car { Id = Guid.NewGuid(), Model = "ModelX", Type = "SUV" };

        _carRepo.ListAllActivesAsync(Arg.Any<CancellationToken>())
                .Returns([car]);

        _carRepo.IsAvailableAsync(car.Id, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
                .Returns(false);

        var query = new CheckAvailabilityQuery(
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(3),
            "SUV", "ModelX");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
