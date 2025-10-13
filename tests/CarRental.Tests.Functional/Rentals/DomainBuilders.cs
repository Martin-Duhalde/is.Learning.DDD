using CarRental.Domain.Entities;

namespace CarRental.Tests.Functional.Rentals;

internal static class DomainBuilders
{
    public static Car BuildCar(Guid? id = null, string model = "ModelX", string type = "SUV")
    {
        return Car.Restore(id ?? Guid.NewGuid(), model, type, isActive: true, version: 1);
    }
}
