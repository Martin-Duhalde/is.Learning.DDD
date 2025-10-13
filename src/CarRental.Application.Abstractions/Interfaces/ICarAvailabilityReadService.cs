/// MIT License (c) 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;

namespace CarRental.Application.Abstractions.Interfaces;

/// <summary>
/// Provides read operations to determine car availability over a period.
/// </summary>
public interface ICarAvailabilityReadService
{
    Task<IReadOnlyList<Car>> ListAvailableAsync(
        string type,
        string model,
        DateTime start,
        DateTime end,
        CancellationToken ct = default);
}
