/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;

namespace CarRental.Application.Abstractions.Repositories;

/// <summary>
/// Repository contract for the Car aggregate.
/// </summary>
public interface ICarRepository
{
    Task<Car?> GetActiveByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Car>> ListAllActivesAsync(CancellationToken ct = default);
    Task AddAsync(Car car, CancellationToken ct = default);
    Task UpdateAsync(Car car, CancellationToken ct = default);
    Task DeleteAsync(Car car, CancellationToken ct = default);
    Task<bool> IsAvailableAsync(Guid carId, DateTime start, DateTime end, CancellationToken ct = default);
    Task<IReadOnlyList<Car>> FindByModelAndTypeAsync(string model, string type, CancellationToken ct = default);
}
