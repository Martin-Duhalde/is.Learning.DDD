/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Application.Rentals.CheckAvailability;

namespace CarRental.Tests.Application.Rentals;

public class CheckAvailabilityQueryHandlerTests
{
    private readonly ICarAvailabilityReadService _availabilityService = Substitute.For<ICarAvailabilityReadService>();
    private readonly CheckAvailabilityQueryHandler _handler;

    public CheckAvailabilityQueryHandlerTests()
    {
        _handler = new CheckAvailabilityQueryHandler(_availabilityService);
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

        _availabilityService.ListAvailableAsync("SUV", "ModelX", start, end, Arg.Any<CancellationToken>())
            .Returns([car1]);

        var query = new CheckAvailabilityQuery(start, end, "SUV", "ModelX");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        await _availabilityService.Received(1).ListAvailableAsync("SUV", "ModelX", start, end, Arg.Any<CancellationToken>());

        Assert.Equal(car1.Id    /**/ , result[0].Id);
        Assert.Equal("SUV"      /**/ , result[0].Type);
        Assert.Equal("ModelX"   /**/ , result[0].Model);
    }

    [Fact]
    public async Task should_return_empty_list_if_no_cars_match_filters()
    {
        // Arrange
        _availabilityService.ListAvailableAsync(
                "SUV",
                "ModelX",
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Car>());

        var query = new CheckAvailabilityQuery(
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2),
            "SUV", "ModelX");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
        await _availabilityService.Received(1).ListAvailableAsync(
            "SUV",
            "ModelX",
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task should_return_empty_list_if_no_car_is_available()
    {
        // Arrange
        _availabilityService.ListAvailableAsync(
                "SUV",
                "ModelX",
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Car>());

        var query = new CheckAvailabilityQuery(
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(3),
            "SUV", "ModelX");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
        await _availabilityService.Received(1).ListAvailableAsync(
            "SUV",
            "ModelX",
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>(),
            Arg.Any<CancellationToken>());
    }
}
