/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.UseCases.Rentals.Create;

namespace CarRental.Tests.Integration.Rentals;

public class CreateRentalTests : IDisposable
{
    private readonly CarRentalDbContext             /**/ _dbContext;
    private readonly EfRentalRepository             /**/ _rentalRepo;
    private readonly EfCarRepository                /**/ _carRepo;
    private readonly CreateRentalCommandHandler     /**/ _handler;

    public CreateRentalTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CarRentalDbContext(options);
        _dbContext.Database.EnsureCreated();

        _rentalRepo /**/ = new EfRentalRepository(_dbContext);
        _carRepo    /**/ = new EfCarRepository(_dbContext);
        _handler    /**/ = new CreateRentalCommandHandler(_rentalRepo, _carRepo);
    }

    [Fact]
    public async Task should_create_rental_with_real_database()
    {
        // Arrange
        var car = new Car
        {
            Id      /**/ = Guid.NewGuid(),
            Type    /**/ = "SUV",
            Model   /**/ = "ModelX"
        };

        _dbContext.Cars.Add(car);
        await _dbContext.SaveChangesAsync();

        var command = new CreateRentalCommand(
            CustomerId:     /**/ Guid.NewGuid(),
            CarId:          /**/ car.Id,
            StartDate:      /**/ DateTime.UtcNow.AddDays(1),
            EndDate:        /**/ DateTime.UtcNow.AddDays(3)
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var rental = await _dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == result);

        Assert.NotNull(rental);
        Assert.Equal(command.CustomerId /**/, rental.CustomerId);
        Assert.Equal(command.CarId      /**/, rental.CarId);
        Assert.Equal(command.StartDate  /**/, rental.StartDate);
        Assert.Equal(command.EndDate    /**/, rental.EndDate);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
