/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.UseCases.Statistics.GetTopCarTypes;

namespace CarRental.Tests.UseCases.Statistics;

public class GetTopCarTypesQueryHandlerTests
{
    private readonly IRentalRepository              /**/ _rentalRepo    /**/ = Substitute.For<IRentalRepository>();
    private readonly ICarRepository                 /**/ _carRepo       /**/ = Substitute.For<ICarRepository>();
    private readonly GetTopCarTypesQueryHandler     /**/ _handler       /**/ ;

    public GetTopCarTypesQueryHandlerTests()
    {
        _handler = new GetTopCarTypesQueryHandler(_rentalRepo, _carRepo);
    }

    [Fact]
    public async Task should_return_empty_if_no_rentals_found()
    {
        // Arrange
        var query = new GetTopCarTypesQuery(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
                   .Returns(new List<Rental>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task should_return_grouped_by_type_and_percentage()
    {
        // Arrange
        var car1   /**/ = new Car { Id = Guid.NewGuid(), Type = "SUV" };
        var car2   /**/ = new Car { Id = Guid.NewGuid(), Type = "Sedan" };
        var car3   /**/ = new Car { Id = Guid.NewGuid(), Type = "Hatch" };

        var rentals /**/ = new List<Rental>
        {
            new Rental { Id = Guid.NewGuid(), Car = car1 },
            new Rental { Id = Guid.NewGuid(), Car = car1 },
            new Rental { Id = Guid.NewGuid(), Car = car2 },
            new Rental { Id = Guid.NewGuid(), Car = car3 }
        };

        var query = new GetTopCarTypesQuery(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow);

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
                   .Returns(rentals);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3          /**/, result.Count);

        Assert.Equal("SUV"      /**/, result[0].Type);
        Assert.Equal(2          /**/, result[0].Count);
        Assert.Equal(50.00      /**/, result[0].Percentage);

        Assert.Equal("Sedan"    /**/, result[1].Type);
        Assert.Equal(1          /**/, result[1].Count);
        Assert.Equal(25.00      /**/, result[1].Percentage);

        Assert.Equal("Hatch"    /**/, result[2].Type);
        Assert.Equal(1          /**/, result[2].Count);
        Assert.Equal(25.00      /**/, result[2].Percentage);
    }

    [Fact]
    public async Task should_return_Unknown_type_when_car_is_null()
    {
        // Arrange
        var rentals = new List<Rental>
        {
            new Rental { Id = Guid.NewGuid(), Car = null }
        };

        var query = new GetTopCarTypesQuery(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow);

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
                   .Returns(rentals);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Unknown"  /**/, result[0].Type);
        Assert.Equal(1          /**/, result[0].Count);
        Assert.Equal(100.00     /**/, result[0].Percentage);
    }

    [Fact]
    public async Task should_return_only_top_3_car_types()
    {
        // Arrange
        var rentals = new List<Rental>();

        for (int i = 1; i <= 10; i++)
        {
            var type    /**/ = $"Type-{i}";
            var car     /**/ = new Car { Id = Guid.NewGuid(), Type = type };
            rentals.Add(new Rental { Id = Guid.NewGuid(), Car = car });
        }

        var query = new GetTopCarTypesQuery(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
                   .Returns(rentals);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3 /**/, result.Count);
    }
}
