/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.UseCases.Rentals.Cancel;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests.Integration.Rentals;

public class CancelRentalCommandHandlerTests
{
    private readonly CarRentalDbContext             /**/ _db;
    private readonly EfRentalRepository             /**/ _rentalRepo;
    private readonly CancelRentalCommandHandler     /**/ _handler;

    public CancelRentalCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new CarRentalDbContext(options);
        _db.Database.EnsureCreated();

        _rentalRepo     /**/ = new EfRentalRepository(_db);
        _handler        /**/ = new CancelRentalCommandHandler(_rentalRepo);
    }

    [Fact]
    public async Task should_cancel_existing_rental_successfully()
    {
        // Arrange
        var rentalId = Guid.NewGuid();

        var rental = new Rental
        {
            Id          /**/ = rentalId,
            CarId       /**/ = Guid.NewGuid(),
            CustomerId  /**/ = Guid.NewGuid(),
            StartDate   /**/ = DateTime.UtcNow.AddDays(1),
            EndDate     /**/ = DateTime.UtcNow.AddDays(3),
            RentalStatus      /**/ = RentalStatus.Active,
            CancelledAt /**/ = null
        };

        await _db.Rentals.AddAsync(rental);
        await _db.SaveChangesAsync();

        var command = new CancelRentalCommand(rentalId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        var cancelledRental = await _db.Rentals.FindAsync(rentalId);
       
        Assert.NotNull(cancelledRental);
        Assert.Equal(RentalStatus.Cancelled, cancelledRental.RentalStatus);
        Assert.NotNull(cancelledRental.CancelledAt);
    }

    [Fact]
    public async Task should_throw_domain_exception_if_rental_not_found()
    {
        // Arrange
        var command = new CancelRentalCommand(Guid.NewGuid());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Rental not found.", ex.Message);
    }
}
