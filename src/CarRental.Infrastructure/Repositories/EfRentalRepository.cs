/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Repositories;
public class EfRentalRepository : EfRepository<Rental>, IRentalRepository
{
    public EfRentalRepository(CarRentalDbContext db) : base(db) { }

    public async Task<List<Rental>> GetRentalsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _db.Rentals
            .Where(r => r.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid carId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await _db.Rentals
            .AnyAsync(r => r.CarId == carId && start <= r.EndDate && end >= r.StartDate, cancellationToken);
    }

    public async Task<List<Rental>> ListActivesBetweenDatesAsync(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        return await _db.Rentals.Include(r => r.Car)
                                .Where(r =>
                                       r.StartDate >= from &&
                                       r.StartDate <= to &&
                                       r.RentalStatus == RentalStatus.Active)
                                .ToListAsync(cancellationToken);
    }

    public async Task<Rental?> GetByIdWithDetailsAsync(Guid rentalId, CancellationToken cancellationToken = default)
    {
        return await _db.Rentals
            .Include(r => r.Car)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == rentalId, cancellationToken);
    }

    public async Task<List<Rental>> ListLast7DaysAsync(CancellationToken cancellationToken = default)
    {
        var today   /**/ = DateTime.UtcNow.Date;
        var weekAgo /**/ = today.AddDays(-6);

        return await _db.Rentals.Where(r =>
                                       r.StartDate.Date >= weekAgo &&
                                       r.StartDate.Date <= today)
                                .ToListAsync(cancellationToken);
    }

    public async Task<List<Rental>> ListCancelledAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await _db.Rentals.Where(r =>
                                       r.RentalStatus == RentalStatus.Cancelled &&
                                       r.CancelledAt >= from &&
                                       r.CancelledAt <= to)
                                .ToListAsync(ct);
    }

    public async Task<int> CountCancelledAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await _db.Rentals
            .Where(r => r.RentalStatus == RentalStatus.Cancelled
                     && r.CancelledAt >= from
                     && r.CancelledAt <= to)
            .CountAsync(ct);
    }

    public async Task CancelAsync(Guid rentalId, CancellationToken ct = default)
    {
        var rental = await _db.Rentals.FindAsync(new object[] { rentalId }, ct);
        if (rental == null) return;

        rental.RentalStatus       /**/ = RentalStatus.Cancelled;
        rental.CancelledAt  /**/ = DateTime.UtcNow;

        _db.Rentals.Update(rental);
        await _db.SaveChangesAsync(ct);
    }
}
