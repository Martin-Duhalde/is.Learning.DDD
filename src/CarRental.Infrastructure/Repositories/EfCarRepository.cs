/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Repositories
{
    public class EfCarRepository : EfRepository<Car>, ICarRepository
    {
        public EfCarRepository(CarRentalDbContext db) : base(db) { }

        public async Task<bool> IsAvailableAsync(
            Guid carId, DateTime start, DateTime end, CancellationToken ct = default)
        {
            // Trae todos los rentals de ese coche en el periodo
            var conflicts = await _db.Rentals
                .Where(r => r.CarId == carId
                         && start <= r.EndDate
                         && end >= r.StartDate
                         && r.Status == RentalStatus.Active )
                .AnyAsync(ct);

            return !conflicts;
        }
    }

}
