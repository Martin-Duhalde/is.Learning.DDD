/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.UseCases.Rentals.List;

namespace CarRental.Tests.UseCases.Rentals;

public class GetAllRentalsQueryHandlerTests
{
    private readonly IRentalRepository _rentalRepo = Substitute.For<IRentalRepository>();
    private readonly GetAllRentalsQueryHandler _handler;

    public GetAllRentalsQueryHandlerTests()
    {
        _handler = new GetAllRentalsQueryHandler(_rentalRepo);
    }

    [Fact]
    public async Task should_return_all_rentals()
    {
        // Arrange
        var rental1 = new Rental
        {
            Id          /**/ = Guid.NewGuid(),
            CustomerId  /**/ = Guid.NewGuid(),
            CarId       /**/ = Guid.NewGuid(),
            StartDate   /**/ = DateTime.UtcNow.AddDays(1),
            EndDate     /**/ = DateTime.UtcNow.AddDays(3)
        };

        var rental2 = new Rental
        {
            Id          /**/ = Guid.NewGuid(),
            CustomerId  /**/ = Guid.NewGuid(),
            CarId       /**/ = Guid.NewGuid(),
            StartDate   /**/ = DateTime.UtcNow.AddDays(4),
            EndDate     /**/ = DateTime.UtcNow.AddDays(6)
        };

        _rentalRepo.ListAllActivesAsync(Arg.Any<CancellationToken>())
                   .Returns([rental1, rental2]);

        var query = new GetAllRentalsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2 /**/, result.Count);

        Assert.Equal(rental1.Id         /**/, result[0].Id);
        Assert.Equal(rental1.CustomerId /**/, result[0].CustomerId);
        Assert.Equal(rental1.CarId      /**/, result[0].CarId);
        Assert.Equal(rental1.StartDate  /**/, result[0].StartDate);
        Assert.Equal(rental1.EndDate    /**/, result[0].EndDate);

        Assert.Equal(rental2.Id         /**/, result[1].Id);
        Assert.Equal(rental2.CustomerId /**/, result[1].CustomerId);
        Assert.Equal(rental2.CarId      /**/, result[1].CarId);
        Assert.Equal(rental2.StartDate  /**/, result[1].StartDate);
        Assert.Equal(rental2.EndDate    /**/, result[1].EndDate);
    }
}
