/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Rentals.Create;

namespace CarRental.Tests.UseCases.Rentals;

public class CreateRentalCommandHandlerTests
{
    private readonly IRentalRepository          /**/ _rentalRepo    /**/ = Substitute.For<IRentalRepository>();
    private readonly ICarRepository             /**/ _carRepo       /**/ = Substitute.For<ICarRepository>();
    private readonly CreateRentalCommandHandler /**/ _handler;

    public CreateRentalCommandHandlerTests()
    {
        _handler = new CreateRentalCommandHandler(_rentalRepo, _carRepo);
    }

    [Fact]
    public async Task should_create_rental_if_car_is_available()
    {
        // Arrange
        var command = new CreateRentalCommand(
            CustomerId:     /**/ Guid.NewGuid(),
            CarId:          /**/ Guid.NewGuid(),
            StartDate:      /**/ DateTime.UtcNow.AddDays(1),
            EndDate:        /**/ DateTime.UtcNow.AddDays(3)
        );

        _carRepo.IsAvailableAsync(command.CarId, command.StartDate, command.EndDate, Arg.Any<CancellationToken>())
                .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        await _rentalRepo.Received(1).AddAsync(Arg.Is<Rental>(r =>
            r.CustomerId    /**/ == command.CustomerId &&
            r.CarId         /**/ == command.CarId &&
            r.StartDate     /**/ == command.StartDate &&
            r.EndDate       /**/ == command.EndDate
        ), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task should_throw_if_car_is_not_available()
    {
        // Arrange
        var command = new CreateRentalCommand(
            CustomerId: /**/ Guid.NewGuid(),
            CarId:      /**/ Guid.NewGuid(),
            StartDate:  /**/ DateTime.UtcNow.AddDays(1),
            EndDate:    /**/ DateTime.UtcNow.AddDays(3)
        );

        _carRepo.IsAvailableAsync(command.CarId, command.StartDate, command.EndDate, Arg.Any<CancellationToken>())
                .Returns(false);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("The car is not available for the selected period.", ex.Message);
    }

}
