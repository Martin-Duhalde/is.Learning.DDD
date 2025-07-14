/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.UseCases.Rentals.Modify;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests.Integration.Rentals;

public class ModifyReservationCommandHandlerTests
{
    private readonly CarRentalDbContext         /**/ _dbContext;
    private readonly ModifyRentalCommandHandler /**/ _handler;

    public ModifyReservationCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CarRentalDbContext(options);

        IRentalRepository   /**/ rentalRepo     /**/ = new EfRentalRepository(_dbContext);
        ICarRepository      /**/ carRepo        /**/ = new EfCarRepository(_dbContext);

        _handler = new ModifyRentalCommandHandler(rentalRepo, carRepo);
    }

    [Fact]
    public async Task should_modify_reservation_if_new_period_is_available()
    {
        // Arrange
        var car = new Car
        {
            Model = "ModelS",
            Type = "Sedan"
        };

        var customer = new Customer
        {
            FullName = "Martín",
            Address = "Argentina"
        };

        var rental = new Rental
        {
            CarId           /**/ = car.Id,
            Car             /**/ = car,
            CustomerId      /**/ = customer.Id,
            Customer        /**/ = customer,
            StartDate       /**/ = DateTime.UtcNow.AddDays(1),
            EndDate         /**/ = DateTime.UtcNow.AddDays(3)
        };

        _dbContext.Cars     /**/ .Add(car);
        _dbContext.Customers/**/ .Add(customer);
        _dbContext.Rentals  /**/ .Add(rental);

        await _dbContext.SaveChangesAsync();  /// Save

        var newStart        /**/ = DateTime.UtcNow.AddDays(5);
        var newEnd          /**/ = DateTime.UtcNow.AddDays(7);

        var command = new ModifyRentalCommand(rental.Id, newStart, newEnd, null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updated = await _dbContext.Rentals.FindAsync(rental.Id);

        Assert.NotNull(updated);

        Assert.Equal(newStart   /**/ , updated!.StartDate);
        Assert.Equal(newEnd     /**/ , updated.EndDate);
        Assert.Equal(car.Id     /**/ , updated.CarId); // no se cambia el auto // same car
    }

    [Fact]
    public async Task should_modify_reservation_and_change_car_if_new_car_is_available()
    {
        // Arrange
        var carOld = new Car { Model = "ModelOld", Type = "Sedan" };
        var carNew = new Car { Model = "ModelNew", Type = "SUV" };

        var customer = new Customer { FullName = "Martín", Address = "Argentina" };

        var rental = new Rental
        {
            CarId               /**/ = carOld.Id,
            Car                 /**/ = carOld,
            CustomerId          /**/ = customer.Id,
            Customer            /**/ = customer,
            StartDate           /**/ = DateTime.UtcNow.AddDays(1),
            EndDate             /**/ = DateTime.UtcNow.AddDays(3)
        };

        _dbContext.Cars         /**/ .AddRange(carOld, carNew);
        _dbContext.Customers    /**/ .Add(customer);
        _dbContext.Rentals      /**/ .Add(rental);

        await _dbContext.SaveChangesAsync();    /// save

        var newStart            /**/ = DateTime.UtcNow.AddDays(5);
        var newEnd              /**/ = DateTime.UtcNow.AddDays(7);

        var command = new ModifyRentalCommand(rental.Id, newStart, newEnd, carNew.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updated = await _dbContext.Rentals.FindAsync(rental.Id);

        Assert.NotNull(updated);

        Assert.Equal(newStart,  /**/ updated!.StartDate);
        Assert.Equal(newEnd,    /**/ updated.EndDate);
        Assert.Equal(carNew.Id, /**/ updated.CarId); // cambio de auto correcto
    }

    [Fact]
    public async Task should_throw_if_new_period_is_not_available()
    {
        // Arrange

        var car = new Car
        {
            Model = "ModelX",
            Type = "SUV"
        };

        var customer = new Customer
        {
            FullName = "Martín",
            Address = "Argentina"
        };

        var rental = new Rental
        {
            CarId               /**/ = car.Id,
            Car                 /**/ = car,
            CustomerId          /**/ = customer.Id,
            Customer            /**/ = customer,
            StartDate           /**/ = DateTime.UtcNow.AddDays(1),
            EndDate             /**/ = DateTime.UtcNow.AddDays(3)
        };

        _dbContext.Cars         /**/ .Add(car);
        _dbContext.Customers    /**/ .Add(customer);
        _dbContext.Rentals      /**/ .Add(rental);

        // Otro alquiler que genera conflicto en la nueva fecha y auto
        var conflictingRental = new Rental
        {
            CarId               /**/ = car.Id,
            CustomerId          /**/ = Guid.NewGuid(),
            StartDate           /**/ = DateTime.UtcNow.AddDays(5),
            EndDate             /**/ = DateTime.UtcNow.AddDays(8)
        };
        
        _dbContext.Rentals.Add(conflictingRental);

        await _dbContext.SaveChangesAsync();    /// save

        var command = new ModifyRentalCommand(rental.Id, DateTime.UtcNow.AddDays(6), DateTime.UtcNow.AddDays(7), null);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("The car is not available for the new selected period.", ex.Message);
    }

    [Fact]
    public async Task should_throw_if_end_date_is_before_start_date()
    {
        // Arrange
        var car = new Car
        {
            Model = "ModelX",
            Type = "SUV"
        };

        var customer = new Customer
        {
            FullName = "Martín",
            Address = "Argentina"
        };

        var rental = new Rental
        {
            CarId           /**/ = car.Id,
            Car             /**/ = car,
            CustomerId      /**/ = customer.Id,
            Customer        /**/ = customer,
            StartDate       /**/ = DateTime.UtcNow.AddDays(1),
            EndDate         /**/ = DateTime.UtcNow.AddDays(3)
        };

        _dbContext.Cars     /**/ .Add(car);
        _dbContext.Customers/**/ .Add(customer);
        _dbContext.Rentals  /**/ .Add(rental);

        await _dbContext.SaveChangesAsync();    /// save

        var command = new ModifyRentalCommand(rental.Id, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(4), null);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("End date must be after start date.", ex.Message);
    }

    [Fact]
    public async Task should_throw_if_rental_not_found()
    {
        // Arrange
        var command = new ModifyRentalCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(7), null);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Rental not found.", ex.Message);
    }

}
