/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Repositories;
using CarRental.Application.Statistics.GetTopCarsByBrandModel;
using CarRental.Domain.Entities;
using CarRental.Tests.Application.TestBuilders;

namespace CarRental.Tests.Application.Statistics;

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
        var query = new GetTopCarsByBrandModelQuery(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow);

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
            .Returns(new List<Rental>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task should_return_grouped_results_by_model_and_type()
    {
        var car1 = DomainBuilder.BuildCar(model: "Model S", type: "Sedan");
        var car2 = DomainBuilder.BuildCar(model: "Model X", type: "SUV");

        var rentals = new List<Rental>
        {
            DomainBuilder.BuildRental(car1),
            DomainBuilder.BuildRental(car1),
            DomainBuilder.BuildRental(car2)
        };

        var query = new GetTopCarsByBrandModelQuery(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
            .Returns(rentals);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count);

        var stat1 = result.First(r => r.Model == "Model S");
        var stat2 = result.First(r => r.Model == "Model X");

        Assert.Equal("Sedan", stat1.Type);
        Assert.Equal(2, stat1.Count);
        Assert.Equal(66.67, stat1.Percentage);

        Assert.Equal("SUV", stat2.Type);
        Assert.Equal(1, stat2.Count);
        Assert.Equal(33.33, stat2.Percentage);
    }

    [Fact]
    public async Task should_return_unknown_for_null_car_data()
    {
        var rentals = new List<Rental>
        {
            DomainBuilder.BuildRental()
        };

        var query = new GetTopCarsByBrandModelQuery(DateTime.UtcNow.AddDays(-3), DateTime.UtcNow);

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
            .Returns(rentals);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Unknown", result[0].Model);
        Assert.Equal("Unknown", result[0].Type);
        Assert.Equal(1, result[0].Count);
        Assert.Equal(100.0, result[0].Percentage);
    }

    [Fact]
    public async Task should_return_only_top_10_results()
    {
        var rentals = new List<Rental>();

        for (int i = 1; i <= 20; i++)
        {
            var car = DomainBuilder.BuildCar(model: $"Model-{i}", type: "Type");
            rentals.Add(DomainBuilder.BuildRental(car));
        }

        var query = new GetTopCarsByBrandModelQuery(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

        _rentalRepo.ListActivesBetweenDatesAsync(query.From, query.To, Arg.Any<CancellationToken>())
            .Returns(rentals);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(10, result.Count);
    }
}
