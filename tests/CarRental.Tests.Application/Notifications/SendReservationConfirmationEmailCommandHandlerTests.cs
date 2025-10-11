/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Interfaces;
using CarRental.Application.Abstractions.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Application.Notifications.SendEmail;

namespace CarRental.Tests.Application.Notifications;

public class SendReservationConfirmationEmailCommandHandlerTests
{
    private readonly IRentalRepository              /**/  _rentalRepo   /**/ = Substitute.For<IRentalRepository>();
    private readonly IEmailService                  /**/ _emailService  /**/ = Substitute.For<IEmailService>();
    private readonly IUserDirectory                 /**/ _userDirectory /**/ = Substitute.For<IUserDirectory>();

    private readonly SendReservationConfirmationEmailCommandHandler _handler;

    public SendReservationConfirmationEmailCommandHandlerTests()
    {
        _handler = new SendReservationConfirmationEmailCommandHandler(
            _rentalRepo,
            _emailService,
            _userDirectory
        );
    }

    [Fact]
    public async Task should_send_email_when_rental_is_valid()
    {
        // Arrange
        var rentalId   /**/ = Guid.NewGuid();
        var customer   /**/ = new Customer
        {
            FullName   /**/ = "John Doe",
            UserId     /**/ = "user-123"
        };
        var car        /**/ = new Car
        {
            Model      /**/ = "Model S",
            Type       /**/ = "Sedan"
        };
        var rental     /**/ = new Rental
        {
            Id         /**/ = rentalId,
            Customer   /**/ = customer,
            Car        /**/ = car,
            StartDate  /**/ = new DateTime(2025, 7, 15),
            EndDate    /**/ = new DateTime(2025, 7, 20)
        };

        _rentalRepo.GetByIdWithDetailsAsync(rentalId, Arg.Any<CancellationToken>())
                   .Returns(rental);
        _userDirectory.GetByIdAsync(customer.UserId, Arg.Any<CancellationToken>())
                      .Returns(new UserDirectoryEntry("johndoe@email.com"));

        var command = new SendReservationConfirmationEmailCommand(rentalId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _emailService.Received(1).SendEmailAsync(
            "johndoe@email.com"       /**/,
            "noreply@carrental.com"   /**/,
            "Car Rental Confirmation" /**/,
            Arg.Is<string>(body =>
                body.Contains("John Doe") &&
                body.Contains("Model S") &&
                body.Contains("Sedan") &&
                body.Contains("2025-07-15") &&
                body.Contains("2025-07-20")
            )
        );
    }

    [Fact]
    public async Task should_throw_if_rental_not_found()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        _rentalRepo.GetByIdWithDetailsAsync(rentalId, Arg.Any<CancellationToken>())
                   .Returns((Rental?)null);

        var command = new SendReservationConfirmationEmailCommand(rentalId);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Rental not found.", ex.Message);
    }

    [Fact]
    public async Task should_throw_when_rental_has_no_car()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var rental = new Rental
        {
            Id = rentalId,
            Customer = new Customer { FullName = "Tom", UserId = "user-456" },
            Car = null
        };

        _rentalRepo.GetByIdWithDetailsAsync(rentalId, Arg.Any<CancellationToken>())
                   .Returns(rental);

        _userDirectory.GetByIdAsync("user-456", Arg.Any<CancellationToken>())
                      .Returns(new UserDirectoryEntry("tom@email.com"));

        var command = new SendReservationConfirmationEmailCommand(rentalId);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Rental Car data is missing.", ex.Message);
    }

    [Fact]
    public async Task should_throw_when_rental_has_no_customer()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var rental = new Rental
        {
            Id = rentalId,
            Customer = null,
            Car = new Car { Model = "Model X", Type = "SUV" }
        };

        _rentalRepo.GetByIdWithDetailsAsync(rentalId, Arg.Any<CancellationToken>())
                   .Returns(rental);

        var command = new SendReservationConfirmationEmailCommand(rentalId);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Rental Customer data is missing.", ex.Message);
    }
}
