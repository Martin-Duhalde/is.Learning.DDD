/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;

using Microsoft.Extensions.Caching.Memory;

namespace CarRental.Infrastructure.Caching;

public class ServiceCachedRepository<T> : IServiceRepository
{
    private readonly IServiceRepository   /**/ _innerRepo;
    private readonly IMemoryCache         /**/ _cache;
    private readonly string               /**/ _cachePrefix;

    public ServiceCachedRepository(IServiceRepository innerRepo, IMemoryCache cache)
    {
        _innerRepo      /**/ = innerRepo;
        _cache          /**/ = cache;
        _cachePrefix    /**/ = typeof(T).Name;
    }

    public async Task<Service?> GetActiveByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"{_cachePrefix}_ById_{id}";
        if (_cache.TryGetValue(cacheKey, out Service? cached))
            return cached;

        var service = await _innerRepo.GetActiveByIdAsync(id, ct);
        if (service != null)
            _cache.Set(cacheKey, service, TimeSpan.FromMinutes(10));
        return service;
    }

    public async Task<IReadOnlyList<Service>> ListAllActivesAsync(CancellationToken ct = default)
    {
        var cacheKey = $"{_cachePrefix}_AllActives";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Service>? cached))
            return cached!;

        var list = await _innerRepo.ListAllActivesAsync(ct);
        _cache.Set(cacheKey, list, TimeSpan.FromMinutes(10));
        return list;
    }

    public async Task<List<(string Model, string Type, DateTime Date)>> GetScheduledServicesAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        // Key segment to group by range (avoid too many permutations)
        var rangeKey = $"{from:yyyyMMdd}_{to:yyyyMMdd}";
        var cacheKey = $"{_cachePrefix}_Scheduled_{rangeKey}";

        if (_cache.TryGetValue(cacheKey, out List<(string, string, DateTime)>? cached) && cached != null)
            return cached;

        var result = await _innerRepo.GetScheduledServicesAsync(from, to, ct);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        return result;
    }

    public async Task<List<Service>> FindActivesByCarAndDateAsync(Guid carId, DateTime date, CancellationToken ct = default)
    {
        var dateKey = date.ToString("yyyyMMdd");
        var cacheKey = $"{_cachePrefix}_Car_{carId}_Date_{dateKey}";

        if (_cache.TryGetValue(cacheKey, out List<Service>? cached) && cached != null)
            return cached;

        var result = await _innerRepo.FindActivesByCarAndDateAsync(carId, date, ct);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        return result;
    }

    public async Task AddAsync(Service entity, CancellationToken ct = default)
    {
        await _innerRepo.AddAsync(entity, ct);
        InvalidateCache(entity.Id);
    }

    public async Task UpdateAsync(Service entity, CancellationToken ct = default)
    {
        await _innerRepo.UpdateAsync(entity, ct);
        InvalidateCache(entity.Id);
    }

    public async Task DeleteAsync(Service entity, CancellationToken ct = default)
    {
        await _innerRepo.DeleteAsync(entity, ct);
        InvalidateCache(entity.Id);
    }

    protected virtual void InvalidateCache(Guid id)
    {
        _cache.Remove($"{_cachePrefix}_AllActives");
        _cache.Remove($"{_cachePrefix}_ById_{id}");

        // ⚠️ También podrías limpiar claves por fechas y rangos si querés máxima consistencia.
    }
}
