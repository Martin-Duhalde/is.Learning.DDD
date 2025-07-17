using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests.Integration.Repositories;

public class EfServiceRepositoryTests
{
    private readonly DbContextOptions<CarRentalDbContext> _options;

    public EfServiceRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task should_return_scheduled_services_between_dates()
    {
        using var context = new CarRentalDbContext(_options);

        var car1 = new Car { Id = Guid.NewGuid(), Model = "Toyota", Type = "Sedan" };
        var car2 = new Car { Id = Guid.NewGuid(), Model = "Ford", Type = "SUV" };

        context.Cars.AddRange(car1, car2);
        context.Services.AddRange(
            new Service { CarId = car1.Id, Car = car1, Date = DateTime.Today.AddDays(-1) },
            new Service { CarId = car2.Id, Car = car2, Date = DateTime.Today },
            new Service { CarId = car2.Id, Car = car2, Date = DateTime.Today.AddDays(2) } // fuera de rango
        );
        await context.SaveChangesAsync();

        var repo = new EfServiceRepository(context);
        var from = DateTime.Today.AddDays(-2);
        var to = DateTime.Today;
        var result = await repo.GetScheduledServicesAsync(from, to);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Model == "Toyota" && s.Type == "Sedan");
        Assert.Contains(result, s => s.Model == "Ford" && s.Type == "SUV");
    }

    [Fact]
    public async Task should_return_actives_by_car_and_date()
    {
        using var context = new CarRentalDbContext(_options);

        var car = new Car { Id = Guid.NewGuid(), Model = "Chevy", Type = "Hatch" };
        var date = DateTime.Today;

        context.Cars.Add(car);
        context.Services.AddRange(
            new Service { CarId = car.Id, Car = car, Date = date, IsActive = true },
            new Service { CarId = car.Id, Car = car, Date = date, IsActive = false },
            new Service { CarId = car.Id, Car = car, Date = date.AddDays(-1), IsActive = true }
        );
        await context.SaveChangesAsync();

        var repo = new EfServiceRepository(context);
        var result = await repo.FindActivesByCarAndDateAsync(car.Id, date);

        Assert.Single(result);
        Assert.True(result.First().IsActive);
        Assert.Equal(date.Date, result.First().Date.Date);
    }
}
