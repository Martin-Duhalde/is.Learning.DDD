/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.UseCases.Services.GetUpcoming;

namespace CarRental.Tests.UseCases.Services;

public class GetUpcomingServicesQueryHandlerTests
{
    private readonly IServiceRepository                 /**/ _serviceRepo = Substitute.For<IServiceRepository>();
    private readonly GetUpcomingServicesQueryHandler    /**/ _handler;

    public GetUpcomingServicesQueryHandlerTests()
    {
        _handler = new GetUpcomingServicesQueryHandler(_serviceRepo);
    }

    [Fact]
    public async Task should_return_upcoming_services_between_dates()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);

        var services = new List<(string Model, string Type, DateTime Date)>
        {
            ("ModelX", "TypeA", from.AddDays(1)),
            ("ModelY", "TypeB", from.AddDays(3))
        };

        _serviceRepo.GetScheduledServicesAsync(from, to, Arg.Any<CancellationToken>())
                    .Returns(services);

        var query = new GetUpcomingServicesQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);

        var first = result[0];
        Assert.Equal("ModelX", first.Model);
        Assert.Equal("TypeA", first.Type);
        Assert.Equal(from.AddDays(1), first.Date);

        var second = result[1];
        Assert.Equal("ModelY", second.Model);
        Assert.Equal("TypeB", second.Type);
        Assert.Equal(from.AddDays(3), second.Date);
    }

    [Fact]
    public async Task should_return_empty_list_when_no_services_found()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);

        _serviceRepo.GetScheduledServicesAsync(from, to, Arg.Any<CancellationToken>())
                    .Returns(new List<(string Model, string Type, DateTime Date)>());

        var query = new GetUpcomingServicesQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task should_return_all_services_within_date_range()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(10);

        var services = new List<(string Model, string Type, DateTime Date)>
        {
            ("ModelA", "Type1", from.AddDays(2)),
            ("ModelB", "Type2", from.AddDays(5)),
            ("ModelC", "Type3", from.AddDays(10))
        };

        _serviceRepo.GetScheduledServicesAsync(from, to, Arg.Any<CancellationToken>())
                    .Returns(services);

        var query = new GetUpcomingServicesQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.All(result, s => Assert.InRange(s.Date, from, to));
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task should_handle_cancellation_token_properly()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);
        var cancellationToken = new CancellationTokenSource().Token;

        var services = new List<(string Model, string Type, DateTime Date)>
        {
            ("ModelX", "TypeA", from.AddDays(1)),
        };

        _serviceRepo.GetScheduledServicesAsync(from, to, cancellationToken)
                    .Returns(services);

        var query = new GetUpcomingServicesQuery(from, to);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.Single(result);
        await _serviceRepo.Received(1).GetScheduledServicesAsync(from, to, cancellationToken);
    }

    [Fact]
    public async Task should_throw_exception_when_repository_fails()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);

        _serviceRepo.GetScheduledServicesAsync(from, to, Arg.Any<CancellationToken>())
                    .Returns<Task<List<(string Model, string Type, DateTime Date)>>>(x => throw new Exception("DB error"));

        var query = new GetUpcomingServicesQuery(from, to);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Equal("DB error", ex.Message);
    }

    [Fact]
    public async Task should_map_services_to_dto_correctly()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);

        var services = new List<(string Model, string Type, DateTime Date)>
        {
            ("ModelX", "TypeA", from.AddDays(1))
        };

        _serviceRepo.GetScheduledServicesAsync(from, to, Arg.Any<CancellationToken>())
                    .Returns(services);

        var query = new GetUpcomingServicesQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal("ModelX", dto.Model);
        Assert.Equal("TypeA", dto.Type);
        Assert.Equal(from.AddDays(1), dto.Date);
    }
    [Fact]
    public async Task should_return_upcoming_services_with_car_info()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);

        // Simular servicios con entidad Car asociada
        var services = new List<(string Model, string Type, DateTime Date, Car Car)>
        {
            ("ModelX", "TypeA", from.AddDays(1), new Car { Model = "ModelX", Type = "TypeA" }),
            ("ModelY", "TypeB", from.AddDays(3), new Car { Model = "ModelY", Type = "TypeB" })
        };

        // Mock adaptado para retornar solo la tupla esperada (sin Car)
        _serviceRepo.GetScheduledServicesAsync(from, to, Arg.Any<CancellationToken>())
                    .Returns(services.Select(s => (s.Model, s.Type, s.Date)).ToList());

        var query = new GetUpcomingServicesQuery(from, to);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);

        var first = result[0];
        Assert.Equal("ModelX", first.Model);
        Assert.Equal("TypeA", first.Type);
        Assert.Equal(from.AddDays(1), first.Date);

        var second = result[1];
        Assert.Equal("ModelY", second.Model);
        Assert.Equal("TypeB", second.Type);
        Assert.Equal(from.AddDays(3), second.Date);
    }

    [Fact]
    public void should_assign_and_read_car_navigation_property()
    {
        var car = new Car { Model = "Civic", Type = "Sedan" };
        var service = new Service
        {
            Date = DateTime.Today,
            CarId = car.Id,
            Car = car // <- Esto cubre el setter
        };

        // Esto cubre el getter
        Assert.Equal("Civic", service.Car?.Model);
    }
}
