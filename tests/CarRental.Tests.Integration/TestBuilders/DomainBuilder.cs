using CarRental.Domain.Entities;

namespace CarRental.Tests.Integration.TestBuilders;

internal static class DomainBuilder
{
    public static Car BuildCar(Guid? id = null, string model = "Model", string type = "Type", bool isActive = true, int version = 1)
    {
        return Car.Restore(id ?? Guid.NewGuid(), model, type, isActive, version);
    }
}
