/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;

using Microsoft.Extensions.Caching.Memory;

namespace CarRental.Infrastructure.Caching
{
    public class CarCachedRepository<T> : ICarRepository
    {
        protected readonly ICarRepository     /**/ _innerRepo;    /// The actual repository (EF-based)
        protected readonly IMemoryCache       /**/ _cache;        /// In-memory cache instance
        protected readonly string             /**/ _cachePrefix;  /// Unique cache key prefix per entity type

        public CarCachedRepository(ICarRepository innerRepo, IMemoryCache cache)
        {
            _innerRepo      /**/ = innerRepo;
            _cache          /**/ = cache;
            _cachePrefix    /**/ = typeof(T).Name;
        }
        public async Task<bool> IsAvailableAsync(Guid carId, DateTime start, DateTime end, CancellationToken ct = default)
        {
            // Not cached: availability can change frequently, better to keep fresh
            return await _innerRepo.IsAvailableAsync(carId, start, end, ct);
        }
        public async Task<IReadOnlyList<Car>> FindByModelAndTypeAsync(string model, string type, CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_Model_{model}_Type_{type}";

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Car>? cachedList))
                return cachedList!;

            var list = await _innerRepo.FindByModelAndTypeAsync(model, type, ct);
            _cache.Set(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }

        public async Task<Car?> GetActiveByIdAsync(Guid id, CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_ById_{id}";

            if (_cache.TryGetValue(cacheKey, out Car? cachedCar))
                return cachedCar;

            var car = await _innerRepo.GetActiveByIdAsync(id, ct);
            if (car != null)
                _cache.Set(cacheKey, car, TimeSpan.FromMinutes(10));

            return car;
        }

        public async Task<IReadOnlyList<Car>> ListAllActivesAsync(CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_AllActives";

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Car>? cachedList))
                return cachedList!;

            var list = await _innerRepo.ListAllActivesAsync(ct);
            _cache.Set(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }

        public async Task AddAsync(Car entity, CancellationToken ct = default)
        {
            await _innerRepo.AddAsync(entity, ct);
            InvalidateCache(entity.Id);
        }

        public async Task UpdateAsync(Car entity, CancellationToken ct = default)
        {
            await _innerRepo.UpdateAsync(entity, ct);
            InvalidateCache(entity.Id);
        }
        public async Task DeleteAsync(Car entity, CancellationToken ct = default)
        {
            await _innerRepo.DeleteAsync(entity, ct);
            InvalidateCache(entity.Id);
        }

        /// <summary>
        /// Elimina entradas de cache relacionadas con esta entidad para mantener consistencia.
        /// </summary>
        protected virtual void InvalidateCache(Guid id)
        {
            _cache.Remove($"{_cachePrefix}_AllActives");
            _cache.Remove($"{_cachePrefix}_ById_{id}");

            // Si cacheás con otras keys, como por modelo y tipo, aquí deberías invalidarlas también.
            // Para simplicidad no están incluidas, pero si tus datos cambian frecuentemente, considera hacerlo.
        }
    }
}