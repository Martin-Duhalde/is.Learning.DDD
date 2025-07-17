using CarRental.Domain.Entities;
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
        var car = new Car { Id = Guid.NewGuid(), Model = "ModelX", Type = "SUV", IsActive = true };
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
        var car = new Car { Id = Guid.NewGuid(), Model = "ModelY", Type = "Sedan", IsActive = true };
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
        context.Cars.Add(new Car
        {
            Id = Guid.NewGuid(),
            Model = "Civic",
            Type = "Hatchback",
            IsActive = true
        });
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
            new Car { Id = Guid.NewGuid(), Model = "Civic", Type = "Hatchback", IsActive = true },
            new Car { Id = Guid.NewGuid(), Model = "Civic", Type = "Hatchback", IsActive = true },
            new Car { Id = Guid.NewGuid(), Model = "Civic", Type = "Hatchback", IsActive = false } // no activo
        );
        await context.SaveChangesAsync();

        var repo = new EfCarRepository(context);

        var results = await repo.FindByModelAndTypeAsync("civic", "HATCHBACK");

        Assert.Equal(2, results.Count);
    }
}
