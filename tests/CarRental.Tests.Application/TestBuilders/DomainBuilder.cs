using CarRental.Domain.Entities;

namespace CarRental.Tests.Application.TestBuilders;

internal static class DomainBuilder
{
    public static Car BuildCar(string model = "Model", string type = "Type", bool isActive = true, int version = 1)
    {
        return Car.Restore(Guid.NewGuid(), model, type, isActive, version);
    }

    public static Rental BuildRental(Car? car = null, Guid? customerId = null)
    {
        return new Rental
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId ?? Guid.NewGuid(),
            CarId = car?.Id ?? Guid.NewGuid(),
            Car = car,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            RentalStatus = RentalStatus.Active,
            IsActive = true,
            Version = 1
        };
    }
}
