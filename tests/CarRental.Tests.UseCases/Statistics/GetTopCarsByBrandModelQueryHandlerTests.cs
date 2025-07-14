/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.UseCases.Statistics.GetTopCarsByBrandModel;

namespace CarRental.Tests.UseCases.Statistics;

public class GetTopCarsByBrandModelQueryHandlerTests
{
    private readonly IRentalRepository _rentalRepo = Substitute.For<IRentalRepository>();
    private readonly GetTopCarsByBrandModelQueryHandler _handler;

    public GetTopCarsByBrandModelQueryHandlerTests()
    {
        _handler = new GetTopCarsByBrandModelQueryHandler(_rentalRepo);
    }

    [Fact]
    public async Task should_return_empty_list_if_no_rentals()
    {
        // Arrange
        var query = new GetTopCarsByBrandModelQuery(
            From: DateTime.UtcNow.AddDays(-10),
            To: DateTime.UtcNow
        );

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
                   .Returns(new List<Rental>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task should_return_grouped_results_by_model_and_type()
    {
        // Arrange
        var query   /**/ = new GetTopCarsByBrandModelQuery(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        var car1    /**/ = new Car { Id = Guid.NewGuid(), Model = "Model S", Type = "Sedan" };
        var car2    /**/ = new Car { Id = Guid.NewGuid(), Model = "Model X", Type = "SUV" };

        var rentals /**/ = new List<Rental>
        {
            new Rental { Id = Guid.NewGuid(), Car = car1 },
            new Rental { Id = Guid.NewGuid(), Car = car1 },
            new Rental { Id = Guid.NewGuid(), Car = car2 }
        };

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
                   .Returns(rentals);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2 /**/, result.Count);

        var stat1 = result.First(r => r.Model == "Model S");
        var stat2 = result.First(r => r.Model == "Model X");

        Assert.Equal("Sedan"  /**/, stat1.Type);
        Assert.Equal(2         /**/, stat1.Count);
        Assert.Equal(66.67     /**/, stat1.Percentage);

        Assert.Equal("SUV"    /**/, stat2.Type);
        Assert.Equal(1         /**/, stat2.Count);
        Assert.Equal(33.33     /**/, stat2.Percentage);
    }

    [Fact]
    public async Task should_return_unknown_for_null_car_data()
    {
        // Arrange
        var query   /**/ = new GetTopCarsByBrandModelQuery(DateTime.UtcNow.AddDays(-3), DateTime.UtcNow);
        var rentals /**/ = new List<Rental>
        {
            new Rental { Id = Guid.NewGuid(), Car = null }
        };

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
                   .Returns(rentals);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Unknown" /**/, result[0].Model);
        Assert.Equal("Unknown" /**/, result[0].Type);
        Assert.Equal(1          /**/, result[0].Count);
        Assert.Equal(100.0      /**/, result[0].Percentage);
    }

    [Fact]
    public async Task should_return_only_top_10_results()
    {
        // Arrange
        var query   /**/ = new GetTopCarsByBrandModelQuery(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
        var rentals /**/ = new List<Rental>();

        for (int i = 1; i <= 20; i++)
        {
            var car = new Car { Id = Guid.NewGuid(), Model = $"Model-{i}", Type = "Type" };
            rentals.Add(new Rental { Id = Guid.NewGuid(), Car = car });
        }

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
                   .Returns(rentals);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(10 /**/, result.Count);
    }
}
