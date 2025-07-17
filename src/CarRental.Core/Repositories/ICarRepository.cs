/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;

namespace CarRental.Core.Repositories;

/// <summary>
/// Repository for Car aggregate.
/// </summary>
public interface ICarRepository : IRepository<Car>
{
    Task<bool>                  /**/ IsAvailableAsync           /**/ (Guid carId, DateTime start, DateTime end, CancellationToken ct = default);
    Task<IReadOnlyList<Car>>    /**/ FindByModelAndTypeAsync    /**/ (string model, string type, CancellationToken ct = default);
}
