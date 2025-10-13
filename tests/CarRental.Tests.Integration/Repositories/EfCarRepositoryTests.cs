using CarRental.Domain.Entities;
using CarRental.Tests.Integration.TestBuilders;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Tests.Integration.Repositories;

public class EfCarRepositoryTests
{
    private readonly DbContextOptions<CarRentalDbContext> _options;

    public EfCarRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task should_return_true_when_car_is_available()
    {
        using var context = new CarRentalDbContext(_options);
        var car = Car.Restore(Guid.NewGuid(), "ModelX", "SUV", isActive: true, version: 1);
        context.Cars.Add(car);
        await context.SaveChangesAsync();

        var repo = new EfCarRepository(context);

        var available = await repo.IsAvailableAsync(car.Id, DateTime.Today, DateTime.Today.AddDays(1));

        Assert.True(available);
    }

    [Fact]
    public async Task should_return_false_when_car_has_conflicting_rental()
    {
        using var context = new CarRentalDbContext(_options);
        var car = Car.Restore(Guid.NewGuid(), "ModelY", "Sedan", isActive: true, version: 1);
        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            CarId = car.Id,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(2),
            IsActive = true,
            RentalStatus = RentalStatus.Active
        };

        context.Cars.Add(car);
        context.Rentals.Add(rental);
        await context.SaveChangesAsync();

        var repo = new EfCarRepository(context);

        var available = await repo.IsAvailableAsync(car.Id, DateTime.Today.AddDays(1), DateTime.Today.AddDays(3));

        Assert.False(available);
    }

    [Fact]
    public async Task should_return_empty_when_model_and_type_do_not_match()
    {
        using var context = new CarRentalDbContext(_options);
        context.Cars.Add(Car.ForTesting(model: "Civic", type: "Hatchback", isActive: true));
        await context.SaveChangesAsync();

        var repo = new EfCarRepository(context);

        var results = await repo.FindByModelAndTypeAsync("ModelX", "SUV");

        Assert.Empty(results);
    }

    [Fact]
    public async Task should_return_matching_cars_when_model_and_type_match()
    {
        using var context = new CarRentalDbContext(_options);
        context.Cars.AddRange(
            Car.Restore(Guid.NewGuid(), "Civic", "Hatchback", isActive: true, version: 1),
            Car.Restore(Guid.NewGuid(), "Civic", "Hatchback", isActive: true, version: 1),
            Car.Restore(Guid.NewGuid(), "Civic", "Hatchback", isActive: false, version: 1) // no activo
        );
        await context.SaveChangesAsync();

        var repo = new EfCarRepository(context);

        var results = await repo.FindByModelAndTypeAsync("civic", "HATCHBACK");

        Assert.Equal(2, results.Count);
    }
}
