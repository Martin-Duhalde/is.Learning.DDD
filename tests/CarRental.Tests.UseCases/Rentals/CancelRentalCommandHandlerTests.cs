/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.UseCases.Rentals.Cancel;

namespace CarRental.Tests.UseCases.Rentals;

public class CancelRentalCommandHandlerTests
{
    private readonly IRentalRepository          /**/ _rentalRepo = Substitute.For<IRentalRepository>();
    private readonly CancelRentalCommandHandler /**/ _handler;

    public CancelRentalCommandHandlerTests()
    {
        _handler = new CancelRentalCommandHandler(_rentalRepo);
    }
    [Fact]
    public async Task should_cancel_existing_rental()
    {
        // Arrange 
        var rentalId = Guid.NewGuid(); /// simulates an existing rental in database

        _rentalRepo.GetActiveByIdAsync(rentalId, Arg.Any<CancellationToken>()) /// NSubstitute Mock Repo IRentalRepository
                   .Returns(new Rental { Id = rentalId });

        var command = new CancelRentalCommand(rentalId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _rentalRepo.Received(1).CancelAsync(rentalId, Arg.Any<CancellationToken>());
    }
    

    [Fact]
    public async Task should_throw_if_rental_not_found()
    {
        // Arrange
        var rentalId /**/ = Guid.NewGuid();

        _rentalRepo.GetActiveByIdAsync(rentalId, Arg.Any<CancellationToken>())
                   .Returns((Rental?)null);

        var command = new CancelRentalCommand(
            RentalId: /**/ rentalId
        );

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Rental not found." /**/, ex.Message);
    }
}
