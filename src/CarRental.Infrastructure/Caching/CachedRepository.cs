///// MIT License © 2025 Martín Duhalde + ChatGPT

//using CarRental.Core.Repositories;
//using CarRental.Domain.Entities;

//using Microsoft.Extensions.Caching.Memory;

//namespace CarRental.Infrastructure.Caching;

///// <summary>
///// Caches repository results using IMemoryCache to improve performance and reduce DB calls.
/////
///// 🧠 How to register this in Program.cs (Dependency Injection):
/////
///// ✅ Option 1 - Manual registration:
/////     services.AddMemoryCache(); // Register memory cache service
/////     services.AddScoped(typeof(EfRepository<>)); // Concrete repo
/////     services.AddScoped(typeof(IRepository<>), provider =>
/////     {
/////         var inner = provider.GetRequiredService<EfRepository<>>();  // Real repository
/////         var cache = provider.GetRequiredService<IMemoryCache>();    // Memory cache
/////         return new CachedRepository<T>(inner, cache);              // Wrap with cache
/////     });
/////
///// ✅ Option 2 - Using Scrutor (decorator library):
/////     dotnet add package Scrutor
/////     services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
/////     services.Decorate(typeof(IRepository<>), typeof(CachedRepository<>));
/////
///// This class decorates any IRepository<T> with caching logic for read operations.
///// Write operations (Add, Update, Delete) invalidate the cache to ensure consistency.
///// </summary>
///// <typeparam name="T">Entity type, must implement IEntity</typeparam>
//public class CachedRepository<T> : IRepository<T> where T : class, IEntity
//{
//    protected readonly IRepository<T>     /**/ _innerRepo;    /// The actual repository (EF-based)
//    protected readonly IMemoryCache       /**/ _cache;        /// In-memory cache instance
//    protected readonly string             /**/ _cachePrefix;  /// Unique cache key prefix per entity type

//    public CachedRepository(IRepository<T> innerRepo, IMemoryCache cache)
//    {
//        _innerRepo      /**/ = innerRepo;
//        _cache          /**/ = cache;
//        _cachePrefix    /**/ = typeof(T).Name;
//    }

//    /// <summary>
//    /// Get all active entities, from cache if available, or from DB and cache the result.
//    /// </summary>
//    public async Task<IReadOnlyList<T>> ListAllActivesAsync(CancellationToken ct = default)
//    {
//        /// Define a cache key using a prefix and a suffix to uniquely identify this query
//        var cacheKey = $"{_cachePrefix}_AllActives";

//        /// Try to get the cached data using the key
//        /// If found and it's of the expected type, return it immediately
//        if (_cache.TryGetValue(cacheKey, out var cachedObj) && cachedObj is IReadOnlyList<T> cached)
//            return cached;

//        /// Otherwise, get the data from the underlying repository (e.g., from the database)
//        var items = await _innerRepo.ListAllActivesAsync(ct);

//        /// Store the result in the cache for 5 minutes
//        _cache.Set(cacheKey, items, TimeSpan.FromMinutes(5));

//        /// Return the freshly retrieved data
//        return items;
//    }

//    public async Task<T?> GetActiveByIdAsync(Guid id, CancellationToken ct = default)
//    {
//        var cacheKey = $"{_cachePrefix}_ById_{id}";

//        if (_cache.TryGetValue(cacheKey, out T? cached))
//            return cached;

//        var entity = await _innerRepo.GetActiveByIdAsync(id, ct);
//        if (entity is not null)
//            _cache.Set(cacheKey, entity, TimeSpan.FromMinutes(10));

//        return entity;
//    }

//    public async Task AddAsync(T entity, CancellationToken ct = default)
//    {
//        await _innerRepo.AddAsync(entity, ct);
//        InvalidateCache(entity.Id);
//    }

//    public async Task UpdateAsync(T entity, CancellationToken ct = default)
//    {
//        await _innerRepo.UpdateAsync(entity, ct);
//        InvalidateCache(entity.Id);
//    }

//    public async Task DeleteAsync(T entity, CancellationToken ct = default)
//    {
//        await _innerRepo.DeleteAsync(entity, ct);
//        InvalidateCache(entity.Id);
//    }
   
//    protected virtual void InvalidateCache(Guid id)
//    {
//        _cache.Remove($"{_cachePrefix}_AllActives");
//        _cache.Remove($"{_cachePrefix}_ById_{id}");
//    }
//}
