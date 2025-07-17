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

    /// <summary>
    /// Recupera servicios existentes para ese mismo auto y fecha (sin incluir los eliminados lógicamente)
    /// </summary>
    /// <param name="carId"></param>
    /// <param name="date"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Service>> FindActivesByCarAndDateAsync(Guid carId, DateTime date, CancellationToken cancellationToken = default);
}