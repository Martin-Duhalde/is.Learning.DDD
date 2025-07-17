/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Rentals.Modify;

namespace CarRental.Tests.UseCases.Rentals;

public class ModifyRentalCommandHandlerTests
{
    private readonly IRentalRepository _rentalRepo = Substitute.For<IRentalRepository>();
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly ModifyRentalCommandHandler _handler;

    public ModifyRentalCommandHandlerTests()
    {
        _handler = new ModifyRentalCommandHandler(_rentalRepo, _carRepo);
    }

    [Fact]
    public async Task should_modify_rental_when_all_conditions_are_valid()
    {
        // Arrange
        var rental = new Rental
        {
            Id          /**/ = Guid.NewGuid(),
            CustomerId  /**/ = Guid.NewGuid(),
            CarId       /**/ = Guid.NewGuid(),
            StartDate   /**/ = DateTime.UtcNow.AddDays(1),
            EndDate     /**/ = DateTime.UtcNow.AddDays(3)
        };

        var newStart    /**/ = DateTime.UtcNow.AddDays(5);
        var newEnd      /**/ = DateTime.UtcNow.AddDays(7);
        var newCarId    /**/ = Guid.NewGuid();

        _rentalRepo.GetActiveByIdAsync(rental.Id, Arg.Any<CancellationToken>())
                   .Returns(rental);

        _carRepo.IsAvailableAsync(newCarId, newStart, newEnd, Arg.Any<CancellationToken>())
                .Returns(true);

        var command = new ModifyRentalCommand(
            RentalId:       /**/ rental.Id,
            NewStartDate:   /**/ newStart,
            NewEndDate:     /**/ newEnd,
            NewCarId:       /**/ newCarId
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newCarId /**/ , rental.CarId);
        Assert.Equal(newStart /**/ , rental.StartDate);
        Assert.Equal(newEnd   /**/ , rental.EndDate);

        await _rentalRepo.Received(1).UpdateAsync(rental, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task should_throw_if_rental_not_found()
    {
        // Arrange
        _rentalRepo.GetActiveByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                   .Returns((Rental?)null);

        var command = new ModifyRentalCommand(
            RentalId:       /**/ Guid.NewGuid(),
            NewStartDate:   /**/ DateTime.UtcNow.AddDays(1),
            NewEndDate:     /**/ DateTime.UtcNow.AddDays(3),
            NewCarId:       /**/ null
        );

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Rental not found." /**/, ex.Message);
    }

    [Fact]
    public async Task should_throw_if_dates_are_invalid()
    {
        // Arrange
        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            CarId = Guid.NewGuid()
        };

        _rentalRepo.GetActiveByIdAsync(rental.Id, Arg.Any<CancellationToken>())
                   .Returns(rental);

        var command = new ModifyRentalCommand(
            RentalId:       /**/ rental.Id,
            NewStartDate:   /**/ DateTime.UtcNow.AddDays(5),
            NewEndDate:     /**/ DateTime.UtcNow.AddDays(4),
            NewCarId:       /**/ null
        );

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("End date must be after start date." /**/, ex.Message);
    }

    [Fact]
    public async Task should_throw_if_car_is_not_available()
    {
        // Arrange
        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            CarId = Guid.NewGuid()
        };

        var newStart    /**/  = DateTime.UtcNow.AddDays(5);
        var newEnd      /**/ = DateTime.UtcNow.AddDays(7);
        var newCarId    /**/ = Guid.NewGuid();

        _rentalRepo.GetActiveByIdAsync(rental.Id, Arg.Any<CancellationToken>())
                   .Returns(rental);

        _carRepo.IsAvailableAsync(newCarId, newStart, newEnd, Arg.Any<CancellationToken>())
                .Returns(false);

        var command = new ModifyRentalCommand(
            RentalId:       /**/ rental.Id,
            NewStartDate:   /**/ newStart,
            NewEndDate:     /**/ newEnd,
            NewCarId:       /**/ newCarId
        );

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("The car is not available for the new selected period." /**/, ex.Message);
    }
}
