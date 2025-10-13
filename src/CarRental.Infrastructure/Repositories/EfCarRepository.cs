using CarRental.Application.Abstractions.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Databases;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Repositories;

public class EfCarRepository : ICarRepository
{
    private readonly CarRentalDbContext _db;

    public EfCarRepository(CarRentalDbContext db)
    {
        _db = db;
    }

    public async Task<Car?> GetActiveByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Cars
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, ct);
    }

    public async Task<IReadOnlyList<Car>> ListAllActivesAsync(CancellationToken ct = default)
    {
        return await _db.Cars
            .Where(c => c.IsActive)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(Car car, CancellationToken ct = default)
    {
        await _db.Cars.AddAsync(car, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Car car, CancellationToken ct = default)
    {
        var existing = await _db.Cars
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == car.Id, ct)
            ?? throw new DomainException("Car not found.");

        if (existing.Version != car.Version)
            throw new ConcurrencyConflictException("The car has been modified by another user or process.");

        car.IncrementVersion();

        _db.Cars.Update(car);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Car car, CancellationToken ct = default)
    {
        var existing = await _db.Cars
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == car.Id, ct)
            ?? throw new DomainException("Car not found.");

        if (!existing.IsActive)
            throw new DomainException("Car is already deleted.");

        if (existing.Version != car.Version)
            throw new ConcurrencyConflictException("The car has been modified by another user or process.");

        car.Deactivate();
        car.IncrementVersion();

        _db.Cars.Update(car);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> IsAvailableAsync(Guid carId, DateTime start, DateTime end, CancellationToken ct = default)
    {
        var conflicts = await _db.Rentals
            .Where(r => r.RentalStatus == RentalStatus.Active &&
                        r.CarId == carId &&
                        start <= r.EndDate &&
                        end >= r.StartDate)
            .AnyAsync(ct);

        return !conflicts;
    }

    public async Task<IReadOnlyList<Car>> FindByModelAndTypeAsync(string model, string type, CancellationToken ct = default)
    {
        var normalizedModel = model.Trim().ToLower();
        var normalizedType = type.Trim().ToLower();

        return await _db.Cars
            .Where(c => c.IsActive &&
                        c.Model.ToLower() == normalizedModel &&
                        c.Type.ToLower() == normalizedType)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
