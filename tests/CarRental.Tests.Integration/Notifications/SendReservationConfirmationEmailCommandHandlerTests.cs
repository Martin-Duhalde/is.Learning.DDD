/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.Application.Notifications.SendEmail;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace CarRental.Tests.Integration.Notifications;

public class SendReservationConfirmationEmailCommandHandlerTests
{
    private readonly CarRentalDbContext                             /**/ _db;
    private readonly EfRentalRepository                             /**/ _rentalRepo;
    private readonly Mock<IEmailService>                            /**/ _emailServiceMock;
    private readonly Mock<IUserDirectory>                           /**/ _userDirectoryMock;
    private readonly SendReservationConfirmationEmailCommandHandler /**/ _handler;

    public SendReservationConfirmationEmailCommandHandlerTests()
    {
        // Configurar DbContext InMemory
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new CarRentalDbContext(options);
        _db.Database.EnsureCreated();

        _rentalRepo = new EfRentalRepository(_db);

        // Mock IEmailService
        _emailServiceMock = new Mock<IEmailService>();
        _userDirectoryMock = new Mock<IUserDirectory>();

        // Inicializar handler con mocks y repo real
        _handler = new SendReservationConfirmationEmailCommandHandler(
            _rentalRepo,
            _emailServiceMock.Object,
            _userDirectoryMock.Object
        );
    }

    [Fact]
    public async Task should_send_email_when_rental_exists_and_data_is_valid()
    {
        // Arrange
        var customerUserId = "user-123";
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FullName = "John Doe",
            UserId = customerUserId
        };

        var car = new Car
        {
            Id = Guid.NewGuid(),
            Model = "Model S",
            Type = "Sedan"
        };

        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Customer = customer,
            CarId = car.Id,
            Car = car,
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(3),
            RentalStatus = RentalStatus.Active
        };

        // Insertar en la DB InMemory
        await _db.Cars.AddAsync(car);
        await _db.Customers.AddAsync(customer);
        await _db.Rentals.AddAsync(rental);
        await _db.SaveChangesAsync();

        // Mock IUserDirectory para devolver usuario con email
        _userDirectoryMock
            .Setup(u => u.GetByIdAsync(customerUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDirectoryEntry("john@example.com"));

        // Act
        var command = new SendReservationConfirmationEmailCommand(rental.Id);
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _emailServiceMock.Verify(
            x => x.SendEmailAsync(
                "john@example.com",
                "noreply@carrental.com",
                "Car Rental Confirmation",
                It.Is<string>(body => body.Contains("John Doe") && body.Contains("Model S") && body.Contains("Sedan"))),
            Times.Once);
    }

    [Fact]
    public async Task should_throw_when_rental_not_found()
    {
        // Arrange
        var command = new SendReservationConfirmationEmailCommand(Guid.NewGuid());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Rental not found.", ex.Message);
    }

    // Caso "Rental sin Customer" se valida en pruebas de UseCases donde el repositorio se stubbea.
    [Fact]
    public async Task should_throw_when_customer_has_no_userid()
    {
        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Customer = new Customer { FullName = "John Doe", UserId = "" },
            Car = new Car { Model = "Model Y", Type = "Sedan" }
        };

        await _db.Rentals.AddAsync(rental);
        await _db.SaveChangesAsync();

        var command = new SendReservationConfirmationEmailCommand(rental.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, default));
        Assert.Equal("Customer UserId is missing.", ex.Message);
    }

    [Fact]
    public async Task should_throw_when_user_not_found()
    {
        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Customer = new Customer { FullName = "Alice", UserId = "user-404" },
            Car = new Car { Model = "Model Z", Type = "Hatch" }
        };

        await _db.Rentals.AddAsync(rental);
        await _db.SaveChangesAsync();

        _userDirectoryMock
            .Setup(u => u.GetByIdAsync("user-404", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDirectoryEntry?)null);

        var command = new SendReservationConfirmationEmailCommand(rental.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, default));
        Assert.Equal("User not found.", ex.Message);
    }

    [Fact]
    public async Task should_throw_when_user_has_no_email()
    {
        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Customer = new Customer { FullName = "Bob", UserId = "user-123" },
            Car = new Car { Model = "Model C", Type = "Coupe" }
        };

        await _db.Rentals.AddAsync(rental);
        await _db.SaveChangesAsync();

        _userDirectoryMock
            .Setup(u => u.GetByIdAsync("user-123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDirectoryEntry(string.Empty));

        var command = new SendReservationConfirmationEmailCommand(rental.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, default));
        Assert.Equal("User email is missing.", ex.Message);
    }

    // Caso "Rental sin Car" se valida en pruebas de UseCases donde el repositorio se stubbea.
}
