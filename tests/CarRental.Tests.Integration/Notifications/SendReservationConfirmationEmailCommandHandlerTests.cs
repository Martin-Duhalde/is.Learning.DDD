/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Auth;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.UseCases.Notifications.SendEmail;

using Microsoft.AspNetCore.Identity;

using Moq;

namespace CarRental.Tests.Integration.Notifications;

public class SendReservationConfirmationEmailCommandHandlerTests
{
    private readonly CarRentalDbContext                             /**/ _db;
    private readonly EfRentalRepository                             /**/ _rentalRepo;
    private readonly Mock<IEmailService>                            /**/ _emailServiceMock;
    private readonly Mock<UserManager<ApplicationUser>>             /**/ _userManagerMock;
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

        // Mock UserManager<ApplicationUser>
        var storeMock = new Mock<IUserStore<ApplicationUser>>();
        var optionsMock = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
        var passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
        var userValidators = new List<IUserValidator<ApplicationUser>> { new Mock<IUserValidator<ApplicationUser>>().Object };
        var passwordValidators = new List<IPasswordValidator<ApplicationUser>> { new Mock<IPasswordValidator<ApplicationUser>>().Object };
        var keyNormalizerMock = new Mock<ILookupNormalizer>();
        var errorsMock = new Mock<IdentityErrorDescriber>();
        var servicesMock = new Mock<IServiceProvider>();
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<ApplicationUser>>>();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            storeMock.Object,
            optionsMock.Object,
            passwordHasherMock.Object,
            userValidators,
            passwordValidators,
            keyNormalizerMock.Object,
            errorsMock.Object,
            servicesMock.Object,
            loggerMock.Object
        );

        // Inicializar handler con mocks y repo real
        _handler = new SendReservationConfirmationEmailCommandHandler(
            _rentalRepo,
            _emailServiceMock.Object,
            _userManagerMock.Object
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

        // Mock UserManager.FindByIdAsync para devolver usuario con email
        var appUser = new ApplicationUser
        {
            Id = customerUserId,
            Email = "john@example.com"
        };

        _userManagerMock
            .Setup(u => u.FindByIdAsync(customerUserId))
            .ReturnsAsync(appUser);

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

    [Fact]
    public async Task should_throw_when_rental_has_no_customer()
    {
        // Arrange
        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Car = new Car { Model = "Model X", Type = "SUV" }
        };

        await _db.Rentals.AddAsync(rental);
        await _db.SaveChangesAsync();

        var command = new SendReservationConfirmationEmailCommand(rental.Id);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, default));
        Assert.Equal("Rental not found.", ex.Message);
    }
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

        _userManagerMock.Setup(u => u.FindByIdAsync("user-404")).ReturnsAsync((ApplicationUser?)null);

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

        var user = new ApplicationUser { Id = "user-123", Email = "" };

        _userManagerMock.Setup(u => u.FindByIdAsync("user-123")).ReturnsAsync(user);

        var command = new SendReservationConfirmationEmailCommand(rental.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, default));
        Assert.Equal("User email is missing.", ex.Message);
    }

    [Fact]
    public async Task should_throw_when_rental_has_no_car()
    {
        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Customer = new Customer { FullName = "Tom", UserId = "u-1" }
        };

        await _db.Rentals.AddAsync(rental);
        await _db.SaveChangesAsync();

        var user = new ApplicationUser { Id = "u-1", Email = "tom@example.com" };

        _userManagerMock.Setup(u => u.FindByIdAsync("u-1")).ReturnsAsync(user);

        var command = new SendReservationConfirmationEmailCommand(rental.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, default));
        Assert.Equal("Rental not found.", ex.Message);
    }
}
