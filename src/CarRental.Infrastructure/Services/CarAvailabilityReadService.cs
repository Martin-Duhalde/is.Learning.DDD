/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Services;

public class CarAvailabilityReadService : ICarAvailabilityReadService
{
    private readonly CarRentalDbContext _dbContext;

    public CarAvailabilityReadService(CarRentalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Car>> ListAvailableAsync(
        string type,
        string model,
        DateTime start,
        DateTime end,
        CancellationToken ct = default)
    {
        var normalizedType = type.ToLower();
        var normalizedModel = model.ToLower();

        return await _dbContext.Cars
            .AsNoTracking()
            .Where(c => c.Type.ToLower() == normalizedType && c.Model.ToLower() == normalizedModel)
            .Where(c => !_dbContext.Rentals.Any(r =>
                r.RentalStatus == RentalStatus.Active &&
                r.CarId == c.Id &&
                start <= r.EndDate &&
                end >= r.StartDate))
            .ToListAsync(ct);
    }
}
