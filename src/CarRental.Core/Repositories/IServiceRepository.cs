/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;

namespace CarRental.Core.Repositories;

/// <summary>
/// Repository for Service aggregate.
/// </summary>
public interface IServiceRepository : IRepository<Service>
{
    /// <summary>
    /// Returns a list of upcoming services within the next X days.
    /// </summary>
    Task<List<(string Model, string Type, DateTime Date)>> GetScheduledServicesAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}