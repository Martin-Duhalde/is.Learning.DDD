/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.UseCases.Statistics.GetDashboard;

namespace CarRental.Tests.Integration.Statistics;

public class GetDashboardDataQueryHandlerTests
{
    private readonly CarRentalDbContext             /**/ _db;
    private readonly EfRentalRepository             /**/ _rentalRepo;
    private readonly GetDashboardDataQueryHandler   /**/ _handler;

    public GetDashboardDataQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db         /**/ = new CarRentalDbContext(options);
        _db.Database.EnsureCreated();

        _rentalRepo = new EfRentalRepository(_db);
        _handler = new GetDashboardDataQueryHandler(_rentalRepo);
    }

    [Fact]
    public async Task should_return_7_day_dashboard_data_with_rental_counts()
    {
        // Arrange
        var today     /**/ = DateTime.UtcNow.Date;

        var rental1 = new Rental
        {
            Id        /**/ = Guid.NewGuid(),
            CarId     /**/ = Guid.NewGuid(),
            StartDate /**/ = today.AddDays(-1),
            EndDate   /**/ = today.AddDays(2),
            RentalStatus    /**/ = RentalStatus.Active
        };

        var rental2 = new Rental
        {
            Id        /**/ = Guid.NewGuid(),
            CarId     /**/ = Guid.NewGuid(),
            StartDate /**/ = today.AddDays(-3),
            EndDate   /**/ = today.AddDays(-2),
            RentalStatus    /**/ = RentalStatus.Active
        };

        await _db.Rentals.AddRangeAsync(rental1, rental2);
        await _db.SaveChangesAsync();

        var query = new GetDashboardDataQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.DailyStats.Count);

        var statDay1 = result.DailyStats.FirstOrDefault(d => d.Date == today.AddDays(-1));
        Assert.NotNull(statDay1);
        Assert.Equal(1, statDay1!.Rentals);

        var statDay3 = result.DailyStats.FirstOrDefault(d => d.Date == today.AddDays(-3));
        Assert.NotNull(statDay3);
        Assert.Equal(1, statDay3!.Rentals);

        var statToday = result.DailyStats.FirstOrDefault(d => d.Date == today);
        Assert.NotNull(statToday);
        Assert.Equal(0, statToday!.Rentals);
    }
      
}

