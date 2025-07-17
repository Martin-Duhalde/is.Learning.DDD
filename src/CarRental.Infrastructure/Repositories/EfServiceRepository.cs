/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Repositories;

public class EfServiceRepository : EfRepository<Service>, IServiceRepository
{
    public EfServiceRepository(CarRentalDbContext db) : base(db) { }

    public async Task<List<(string Model, string Type, DateTime Date)>> GetScheduledServicesAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _db.Services
            .Where(s => s.Date >= from && s.Date <= to)
            .Include(s => s.Car!)
            .Select(s => new ValueTuple<string, string, DateTime>(
                s.Car!.Model,
                s.Car.Type,
                s.Date
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Service>> FindActivesByCarAndDateAsync(Guid carId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _db.Services
            .Where(s => s.CarId == carId && s.Date.Date == date.Date && s.IsActive)
            .ToListAsync(cancellationToken);
    }
}