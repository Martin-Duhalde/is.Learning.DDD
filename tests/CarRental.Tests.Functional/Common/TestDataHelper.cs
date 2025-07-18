using CarRental.Infrastructure.Databases;
using Microsoft.Extensions.DependencyInjection;

public static class TestDataHelper
{
    public static async Task ClearCarsByModelAndTypeAsync(IServiceProvider sp, string model, string type)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();

        var cars = db.Cars.Where(c => c.Model == model && c.Type == type);

        db.Cars.RemoveRange(cars);
        await db.SaveChangesAsync();
    }
}

