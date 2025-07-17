/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.UseCases.Statistics.GetDailyUsage;

namespace CarRental.Tests.UseCases.Statistics;

public class GetDailyUsageQueryHandlerTests
{
    private readonly IRentalRepository _rentalRepo  /**/ = Substitute.For<IRentalRepository>();
    private readonly ICarRepository _carRepo        /**/ = Substitute.For<ICarRepository>();

    private readonly GetDailyUsageQueryHandler _handler;

    public GetDailyUsageQueryHandlerTests()
    {
        _handler = new GetDailyUsageQueryHandler(_rentalRepo, _carRepo);
    }

    [Fact]
    public async Task should_return_stats_for_last_7_days()
    {
        // Arrange
        var today           /**/ = DateTime.UtcNow.Date;
        var rentals         /**/ = new List<Rental>
        {
            new Rental
            {
                Id          /**/ = Guid.NewGuid(),
                CarId       /**/ = Guid.NewGuid(),
                StartDate   /**/ = today.AddDays(-1),
                EndDate     /**/ = today.AddDays(2)
            },
            new Rental
            {
                Id          /**/ = Guid.NewGuid(),
                CarId       /**/ = Guid.NewGuid(),
                StartDate   /**/ = today.AddDays(-3),
                EndDate     /**/ = today.AddDays(-1)
            }
        };

        var cars            /**/ = new List<Car>
        {
            new Car { Id = Guid.NewGuid(), Model = "ModelA", Type = "SUV" },
            new Car { Id = Guid.NewGuid(), Model = "ModelB", Type = "Sedan" },
            new Car { Id = Guid.NewGuid(), Model = "ModelC", Type = "Hatch" }
        };

        _rentalRepo /**/ .ListLast7DaysAsync    /**/ (Arg.Any<CancellationToken>()).Returns(rentals);
        _carRepo    /**/ .ListAllActivesAsync          /**/ (Arg.Any<CancellationToken>()).Returns(cars);

        var command = new GetDailyUsageQuery();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(7, result.Count);

        var statDay1 = result.FirstOrDefault(r => r.Date == today.AddDays(-1));
        Assert.NotNull(statDay1);
        Assert.Equal(1         /**/ , statDay1!.Rentals);
        Assert.Equal(2         /**/ , statDay1.UnusedCars); // 3 cars total - 1 used

        var statDay3 = result.FirstOrDefault(r => r.Date == today.AddDays(-3));
        Assert.NotNull(statDay3);
        Assert.Equal(1         /**/ , statDay3!.Rentals);
        Assert.Equal(2         /**/ , statDay3.UnusedCars);

        var statToday = result.FirstOrDefault(r => r.Date == today);
        Assert.NotNull(statToday);
        Assert.Equal(0         /**/ , statToday!.Rentals);
        Assert.Equal(3         /**/ , statToday.UnusedCars);
    }
}
