/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Databases;

using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Repositories;
public class EfRepository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly CarRentalDbContext _db;

    public EfRepository(CarRentalDbContext db) => _db = db;

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        entity.Version = 1;                         /// Initialize version on add
        entity.IsActive = true;                      /// Default status on add
        await _db.Set<T>().AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        // Retrieve the current entity from the database
        var existing = await _db.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entity.Id, ct);

        if (existing == null)       /**/ throw new DomainException("Entity not found");
        if (!existing.IsActive)     /**/ throw new DomainException("Entity already deleted.");

        // Soft delete: mark status as Deleted and increment version
        entity.IsActive = false;
        entity.Version++;
        
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        // Retrieve the current entity from the database
        var existing = await _db.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entity.Id, ct);

        if (existing == null)       /**/ throw new DomainException("Entity not found");

        // Check for concurrency conflict
        if (existing.Version != entity.Version)
            throw new DomainException("Entity was modified by another process. (Concurrency Exception)");

        /// Increment version on update
        entity.Version++;

        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<T>> ListAllActivesAsync(CancellationToken ct = default)
    {
        // Return only non-deleted entities
        return await _db.Set<T>()
            .Where(e => e.IsActive)
            .ToListAsync(ct);
    }

    public async Task<T?> GetActiveByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Set<T>().FindAsync(new object[] { id }, ct);

        /// Filter out deleted entities here
        if (entity is null || !entity.IsActive)
            return null;

        return entity;
    }
}
