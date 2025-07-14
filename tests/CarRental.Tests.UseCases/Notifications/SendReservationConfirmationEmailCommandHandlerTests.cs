/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Interfaces;
using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Auth;
using CarRental.UseCases.Notifications.SendEmail;

using Microsoft.AspNetCore.Identity;

namespace CarRental.Tests.UseCases.Notifications;

public class SendReservationConfirmationEmailCommandHandlerTests
{
    private readonly IRentalRepository              /**/  _rentalRepo   /**/ = Substitute.For<IRentalRepository>();
    private readonly IEmailService                  /**/ _emailService  /**/ = Substitute.For<IEmailService>();
    private readonly UserManager<ApplicationUser>   /**/ _userManager   /**/;

    private readonly SendReservationConfirmationEmailCommandHandler _handler;

    public SendReservationConfirmationEmailCommandHandlerTests()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);

        _handler = new SendReservationConfirmationEmailCommandHandler(
            _rentalRepo,
            _emailService,
            _userManager
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

        var user       /**/ = new ApplicationUser
        {
            Id         /**/ = customer.UserId,
            Email      /**/ = "johndoe@email.com"
        };

        _rentalRepo.GetByIdWithDetailsAsync(rentalId, Arg.Any<CancellationToken>())
                   .Returns(rental);
        _userManager.FindByIdAsync(customer.UserId)
                    .Returns(user);

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
}
