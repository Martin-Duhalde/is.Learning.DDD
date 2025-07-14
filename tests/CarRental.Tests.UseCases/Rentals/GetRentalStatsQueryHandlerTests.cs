/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.UseCases.Rentals.Get;

namespace CarRental.Tests.UseCases.Rentals;

public class GetRentalStatsQueryHandlerTests
{
    private readonly IRentalRepository _rentalRepo = Substitute.For<IRentalRepository>();
    private readonly GetRentalStatsQueryHandler _handler;

    public GetRentalStatsQueryHandlerTests()
    {
        _handler = new GetRentalStatsQueryHandler(_rentalRepo);
    }

    [Fact]
    public async Task should_return_stats_grouped_by_car_type()
    {
        // Arrange
        var from    /**/ = DateTime.UtcNow.AddDays(-7);
        var to      /**/ = DateTime.UtcNow;

        var rentals = new List<Rental>
        {
            new() { Car = new Car { Type = "SUV" } },
            new() { Car = new Car { Type = "SUV" } },
            new() { Car = new Car { Type = "Sedan" } }
        };

        _rentalRepo.ListActivesBetweenDatesAsync(from, to, Arg.Any<CancellationToken>())
                   .Returns(rentals);

        var query = new GetRentalStatsQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2         /**/, result.Count);

        Assert.Equal("SUV"     /**/, result[0].CarType);
        Assert.Equal(2         /**/, result[0].Count);
        Assert.Equal(66.67     /**/, result[0].Percentage);

        Assert.Equal("Sedan"   /**/, result[1].CarType);
        Assert.Equal(1         /**/, result[1].Count);
        Assert.Equal(33.33     /**/, result[1].Percentage);
    }

    [Fact]
    public async Task should_return_empty_list_if_no_rentals()
    {
        // Arrange
        var from    /**/ = DateTime.UtcNow.AddDays(-7);
        var to      /**/ = DateTime.UtcNow;

        _rentalRepo.ListActivesBetweenDatesAsync(from, to, Arg.Any<CancellationToken>())
                   .Returns(new List<Rental>());

        var query = new GetRentalStatsQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
