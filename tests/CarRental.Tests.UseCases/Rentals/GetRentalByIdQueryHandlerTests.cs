/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Rentals.Get;

namespace CarRental.Tests.UseCases.Rentals;

public class GetRentalByIdQueryHandlerTests
{
    private readonly IRentalRepository _rentalRepo = Substitute.For<IRentalRepository>();
    private readonly GetRentalByIdQueryHandler _handler;

    public GetRentalByIdQueryHandlerTests()
    {
        _handler = new GetRentalByIdQueryHandler(_rentalRepo);
    }

    [Fact]
    public async Task should_return_rental_when_found()
    {
        // Arrange
        var rental = new Rental
        {
            Id          /**/ = Guid.NewGuid(),
            CustomerId  /**/ = Guid.NewGuid(),
            CarId       /**/ = Guid.NewGuid(),
            StartDate   /**/ = DateTime.UtcNow.AddDays(2),
            EndDate     /**/ = DateTime.UtcNow.AddDays(5)
        };

        _rentalRepo.GetActiveByIdAsync(rental.Id, Arg.Any<CancellationToken>())
                   .Returns(rental);

        var query = new GetRentalByIdQuery(rental.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(rental.Id         /**/, result.Id);
        Assert.Equal(rental.CustomerId /**/, result.CustomerId);
        Assert.Equal(rental.CarId      /**/, result.CarId);
        Assert.Equal(rental.StartDate  /**/, result.StartDate);
        Assert.Equal(rental.EndDate    /**/, result.EndDate);
    }

    [Fact]
    public async Task should_throw_if_rental_not_found()
    {
        // Arrange
        var rentalId = Guid.NewGuid();

        _rentalRepo.GetActiveByIdAsync(rentalId, Arg.Any<CancellationToken>())
                   .Returns((Rental?)null);

        var query = new GetRentalByIdQuery(rentalId);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(query, CancellationToken.None));

        Assert.Equal("Rental not found." /**/, ex.Message);
    }
}
