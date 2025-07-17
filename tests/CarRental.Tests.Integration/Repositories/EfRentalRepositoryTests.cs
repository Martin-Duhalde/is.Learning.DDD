using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;

namespace CarRental.Tests.Integration.Repositories;

public class EfRentalRepositoryTests
{
    private readonly DbContextOptions<CarRentalDbContext> _options;

    public EfRentalRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task should_return_rentals_by_customer_id()
    {
        using var context = new CarRentalDbContext(_options);
        var customerId = Guid.NewGuid();

        context.Rentals.AddRange(
            new Rental { CustomerId = customerId },
            new Rental { CustomerId = customerId },
            new Rental { CustomerId = Guid.NewGuid() } // otro cliente
        );
        await context.SaveChangesAsync();

        var repo = new EfRentalRepository(context);
        var rentals = await repo.GetRentalsByCustomerIdAsync(customerId);

        Assert.Equal(2, rentals.Count);
    }

    [Fact]
    public async Task should_return_true_if_rental_exists_in_period()
    {
        using var context = new CarRentalDbContext(_options);
        var carId = Guid.NewGuid();

        context.Rentals.Add(new Rental
        {
            CarId = carId,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(2)
        });
        await context.SaveChangesAsync();

        var repo = new EfRentalRepository(context);
        var exists = await repo.ExistsAsync(carId, DateTime.Today.AddDays(1), DateTime.Today.AddDays(3));

        Assert.True(exists);
    }

    [Fact]
    public async Task should_return_false_if_rental_does_not_exist_in_period()
    {
        using var context = new CarRentalDbContext(_options);
        var carId = Guid.NewGuid();

        context.Rentals.Add(new Rental
        {
            CarId = carId,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(2)
        });
        await context.SaveChangesAsync();

        var repo = new EfRentalRepository(context);
        var exists = await repo.ExistsAsync(carId, DateTime.Today.AddDays(5), DateTime.Today.AddDays(6));

        Assert.False(exists);
    }

    [Fact]
    public async Task should_return_rentals_started_last_7_days()
    {
        using var context = new CarRentalDbContext(_options);
        context.Rentals.AddRange(
            new Rental { StartDate = DateTime.UtcNow.AddDays(-1) },
            new Rental { StartDate = DateTime.UtcNow.AddDays(-6) },
            new Rental { StartDate = DateTime.UtcNow.AddDays(-8) } // fuera del rango
        );
        await context.SaveChangesAsync();

        var repo = new EfRentalRepository(context);
        var result = await repo.ListLast7DaysAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task should_return_cancelled_rentals_between_dates()
    {
        using var context = new CarRentalDbContext(_options);
        context.Rentals.AddRange(
            new Rental { RentalStatus = RentalStatus.Cancelled, CancelledAt = DateTime.Today },
            new Rental { RentalStatus = RentalStatus.Cancelled, CancelledAt = DateTime.Today.AddDays(-1) },
            new Rental { RentalStatus = RentalStatus.Active, CancelledAt = null }
        );
        await context.SaveChangesAsync();

        var repo = new EfRentalRepository(context);
        var result = await repo.ListCancelledAsync(DateTime.Today.AddDays(-2), DateTime.Today);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task should_count_cancelled_rentals_between_dates()
    {
        using var context = new CarRentalDbContext(_options);
        context.Rentals.AddRange(
            new Rental { RentalStatus = RentalStatus.Cancelled, CancelledAt = DateTime.Today },
            new Rental { RentalStatus = RentalStatus.Cancelled, CancelledAt = DateTime.Today.AddDays(-1) },
            new Rental { RentalStatus = RentalStatus.Active }
        );
        await context.SaveChangesAsync();

        var repo = new EfRentalRepository(context);
        var count = await repo.CountCancelledAsync(DateTime.Today.AddDays(-2), DateTime.Today);

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task should_cancel_rental_and_set_cancelled_at()
    {
        using var context = new CarRentalDbContext(_options);
        var rentalId = Guid.NewGuid();

        context.Rentals.Add(new Rental { Id = rentalId });
        await context.SaveChangesAsync();

        var repo = new EfRentalRepository(context);
        await repo.CancelAsync(rentalId);

        var rental = await context.Rentals.FindAsync(rentalId);

        Assert.Equal(RentalStatus.Cancelled, rental!.RentalStatus);
        Assert.NotNull(rental.CancelledAt);
    }

    [Fact]
    public async Task should_do_nothing_if_rental_to_cancel_not_found()
    {
        using var context = new CarRentalDbContext(_options);
        var repo = new EfRentalRepository(context);

        // No exception should be thrown even if ID doesn't exist
        var act = async () => await repo.CancelAsync(Guid.NewGuid());
        await act();
    }
}
