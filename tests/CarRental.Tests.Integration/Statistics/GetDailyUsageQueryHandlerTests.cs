/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.UseCases.Statistics.GetDailyUsage;

namespace CarRental.Tests.Integration.Statistics;

public class GetDailyUsageQueryHandlerTests
{
    private readonly CarRentalDbContext             /**/ _db;
    private readonly EfRentalRepository             /**/ _rentalRepo;
    private readonly EfCarRepository                /**/ _carRepo;
    private readonly GetDailyUsageQueryHandler      /**/ _handler;

    public GetDailyUsageQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new CarRentalDbContext(options);
        _db.Database.EnsureCreated();

        _rentalRepo /**/ = new EfRentalRepository(_db);
        _carRepo    /**/ = new EfCarRepository(_db);
        _handler    /**/ = new GetDailyUsageQueryHandler(_rentalRepo, _carRepo);
    }

    [Fact]
    public async Task should_return_stats_for_last_7_days_with_rentals_and_cancellations_and_unused()
    {
        // Arrange
        var today   /**/ = DateTime.UtcNow.Date;

        var car1    /**/ = new Car { Id = Guid.NewGuid(), Model = "Model A", Type = "SUV" };
        var car2    /**/ = new Car { Id = Guid.NewGuid(), Model = "Model B", Type = "Sedan" };
        var car3    /**/ = new Car { Id = Guid.NewGuid(), Model = "Model C", Type = "Hatch" };

        await _db.Cars.AddRangeAsync(car1, car2, car3);

        var rental1 = new Rental
        {
            Id        /**/ = Guid.NewGuid(),
            CarId     /**/ = car1.Id,
            StartDate /**/ = today.AddDays(-1),
            EndDate   /**/ = today.AddDays(2),
            RentalStatus    /**/ = RentalStatus.Active
        };

        var rental2 = new Rental
        {
            Id        /**/ = Guid.NewGuid(),
            CarId     /**/ = car2.Id,
            StartDate /**/ = today.AddDays(-3),
            EndDate   /**/ = today.AddDays(-1),
            RentalStatus    /**/ = RentalStatus.Cancelled
        };

        await _db.Rentals.AddRangeAsync(rental1, rental2);
        await _db.SaveChangesAsync();

        var todos = await _db.Rentals.ToListAsync();

        var query = new GetDailyUsageQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(7, result.Count);

        var dayMinus1 = today.AddDays(-1);
        var dayMinus3 = today.AddDays(-3);

        var statDay1 = result.FirstOrDefault(d => d.Date == dayMinus1);
        Assert.NotNull(statDay1);
        Assert.Equal(1, statDay1!.Rentals);
        Assert.Equal(0, statDay1.Cancellations);
        Assert.Equal(2, statDay1.UnusedCars);

        var statDay3 = result.FirstOrDefault(d => d.Date == dayMinus3);
        Assert.NotNull(statDay3);
        Assert.Equal(0, statDay3!.Rentals);
        Assert.Equal(1, statDay3.Cancellations);
        Assert.Equal(2, statDay3.UnusedCars);

        var statToday = result.FirstOrDefault(d => d.Date == today);
        Assert.NotNull(statToday);
        Assert.Equal(0, statToday!.Rentals);
        Assert.Equal(0, statToday.Cancellations);
        Assert.Equal(3, statToday.UnusedCars);
    }
}
