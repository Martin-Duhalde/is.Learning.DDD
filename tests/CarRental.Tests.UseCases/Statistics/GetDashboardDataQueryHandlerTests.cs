/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.UseCases.Statistics.GetDashboard;

namespace  CarRental.Tests.UseCases.Statistics;

public class GetDashboardDataQueryHandlerTests
{
    private readonly IRentalRepository              /**/ _rentalRepo = Substitute.For<IRentalRepository>();
    private readonly GetDashboardDataQueryHandler   /**/ _handler;

    public GetDashboardDataQueryHandlerTests()
    {
        _handler = new GetDashboardDataQueryHandler(_rentalRepo);
    }

    [Fact]
    public async Task should_return_7_days_even_with_no_rentals()
    {
        // Arrange
        _rentalRepo.ListLast7DaysAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Rental>());

        var query = new GetDashboardDataQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(7 /**/, result.DailyStats.Count);
        Assert.All(result.DailyStats, stat =>
        {
            Assert.Equal(0 /**/, stat.Rentals);
            Assert.Equal(0 /**/, stat.UnusedCars);
        });
    }

    [Fact]
    public async Task should_group_rentals_by_day_correctly()
    {
        // Arrange
        var today       /**/ = DateTime.UtcNow.Date;
        var rentals     /**/ = new List<Rental>
        {
            new Rental { Id = Guid.NewGuid(), CarId = Guid.NewGuid(), StartDate = today },
            new Rental { Id = Guid.NewGuid(), CarId = Guid.NewGuid(), StartDate = today },
            new Rental { Id = Guid.NewGuid(), CarId = Guid.NewGuid(), StartDate = today.AddDays(-1) },
            new Rental { Id = Guid.NewGuid(), CarId = Guid.NewGuid(), StartDate = today.AddDays(-3) },
        };

        _rentalRepo.ListLast7DaysAsync(Arg.Any<CancellationToken>()).Returns(rentals);

        var query = new GetDashboardDataQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(7 /**/, result.DailyStats.Count);

        var statToday = result.DailyStats.FirstOrDefault(x => x.Date == today);
        Assert.NotNull(statToday);
        Assert.Equal(2 /**/, statToday!.Rentals);
        Assert.Equal(0 /**/, statToday.UnusedCars);

        var statYesterday = result.DailyStats.FirstOrDefault(x => x.Date == today.AddDays(-1));
        Assert.NotNull(statYesterday);
        Assert.Equal(1 /**/, statYesterday!.Rentals);

        var stat3DaysAgo = result.DailyStats.FirstOrDefault(x => x.Date == today.AddDays(-3));
        Assert.NotNull(stat3DaysAgo);
        Assert.Equal(1 /**/, stat3DaysAgo!.Rentals);
    }

    [Fact]
    public async Task should_return_zero_for_days_without_rentals()
    {
        // Arrange
        var today       /**/ = DateTime.UtcNow.Date;
        var rentals     /**/ = new List<Rental>
        {
            new Rental { Id = Guid.NewGuid(), CarId = Guid.NewGuid(), StartDate = today }
        };

        _rentalRepo.ListLast7DaysAsync(Arg.Any<CancellationToken>()).Returns(rentals);

        var query = new GetDashboardDataQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var otherDay = result.DailyStats.FirstOrDefault(x => x.Date == today.AddDays(-2));
        Assert.NotNull(otherDay);
        Assert.Equal(0 /**/, otherDay!.Rentals);
    }
}
