/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;

using Microsoft.Extensions.Caching.Memory;

namespace CarRental.Infrastructure.Caching
{
    public class RentalCachedRepository<T> : IRentalRepository
    {
        protected readonly IRentalRepository    /**/ _innerRepo;
        protected readonly IMemoryCache         /**/ _cache;
        protected readonly string               /**/ _cachePrefix;

        public RentalCachedRepository(IRentalRepository innerRepo, IMemoryCache cache)
        {
            _innerRepo   /**/ = innerRepo;
            _cache       /**/ = cache;
            _cachePrefix /**/ = typeof(T).Name;
        }

        public async Task<bool> ExistsAsync(Guid carId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            // No cacheamos porque puede cambiar rápido
            return await _innerRepo.ExistsAsync(carId, start, end, cancellationToken);
        }

        public async Task<Rental?> GetByIdWithDetailsAsync(Guid rentalId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{_cachePrefix}_WithDetails_{rentalId}";

            if (_cache.TryGetValue(cacheKey, out Rental? cached))
                return cached;

            var rental = await _innerRepo.GetByIdWithDetailsAsync(rentalId, cancellationToken);
            if (rental != null)
                _cache.Set(cacheKey, rental, TimeSpan.FromMinutes(10));

            return rental;
        }
        public async Task<Rental?> GetActiveByIdAsync(Guid id, CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_ById_{id}";

            if (_cache.TryGetValue(cacheKey, out Rental? cached))
                return cached;

            var entity = await _innerRepo.GetActiveByIdAsync(id, ct);
            if (entity != null)
                _cache.Set(cacheKey, entity, TimeSpan.FromMinutes(10));

            return entity;
        }

        public async Task<IReadOnlyList<Rental>> ListAllActivesAsync(CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_AllActives";

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Rental>? cachedList))
                return cachedList!;

            var list = await _innerRepo.ListAllActivesAsync(ct);
            _cache.Set(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }

        public async Task<List<Rental>> GetRentalsByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_Customer_{customerId}";

            if (_cache.TryGetValue(cacheKey, out List<Rental>? cached) && cached != null)
                return cached;

            var list = await _innerRepo.GetRentalsByCustomerIdAsync(customerId, ct);
            _cache.Set(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }

        public async Task<List<Rental>> ListActivesBetweenDatesAsync(DateTime from, DateTime to, CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_ActivesBetween_{from:yyyyMMdd}_{to:yyyyMMdd}";

            if (_cache.TryGetValue(cacheKey, out List<Rental>? cached) && cached != null)
                return cached;

            var list = await _innerRepo.ListActivesBetweenDatesAsync(from, to, ct);
            _cache.Set(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }

        public async Task<List<Rental>> ListLast7DaysAsync(CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_Last7Days";

            if (_cache.TryGetValue(cacheKey, out List<Rental>? cached) && cached != null)
                return cached;

            var list = await _innerRepo.ListLast7DaysAsync(ct);
            _cache.Set(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }

        public async Task<List<Rental>> ListCancelledAsync(DateTime from, DateTime to, CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_Cancelled_{from:yyyyMMdd}_{to:yyyyMMdd}";

            if (_cache.TryGetValue(cacheKey, out List<Rental>? cached) && cached != null)
                return cached;

            var list = await _innerRepo.ListCancelledAsync(from, to, ct);
            _cache.Set(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }

        public async Task<int> CountCancelledAsync(DateTime from, DateTime to, CancellationToken ct = default)
        {
            var cacheKey = $"{_cachePrefix}_CountCancelled_{from:yyyyMMdd}_{to:yyyyMMdd}";

            if (_cache.TryGetValue(cacheKey, out int cached))
                return cached;

            var count = await _innerRepo.CountCancelledAsync(from, to, ct);
            _cache.Set(cacheKey, count, TimeSpan.FromMinutes(10));
            return count;
        }

        public async Task CancelAsync(Guid rentalId, CancellationToken ct = default)
        {
            await _innerRepo.CancelAsync(rentalId, ct);
            InvalidateCache(rentalId);
        }

        public async Task AddAsync(Rental entity, CancellationToken ct = default)
        {
            await _innerRepo.AddAsync(entity, ct);
            InvalidateCache(entity.Id);
        }

        public async Task UpdateAsync(Rental entity, CancellationToken ct = default)
        {
            await _innerRepo.UpdateAsync(entity, ct);
            InvalidateCache(entity.Id);
        }

        public async Task DeleteAsync(Rental entity, CancellationToken ct = default)
        {
            await _innerRepo.DeleteAsync(entity, ct);
            InvalidateCache(entity.Id);
        }

        protected virtual void InvalidateCache(Guid id)
        {
            // Por simplicidad, invalidamos todas las caches relacionadas,
            // en caso de datos por rango, sería más complejo y depende del patrón se desee usar.
            _cache.Remove($"{_cachePrefix}_AllActives");
            _cache.Remove($"{_cachePrefix}_ById_{id}");
            _cache.Remove($"{_cachePrefix}_WithDetails_{id}");
            _cache.Remove($"{_cachePrefix}_Last7Days");
        }

    }
}

