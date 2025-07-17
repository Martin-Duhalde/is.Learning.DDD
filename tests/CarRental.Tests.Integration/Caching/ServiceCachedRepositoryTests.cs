/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Caching;

using Microsoft.Extensions.Caching.Memory;

using Moq;

namespace CarRental.Tests.Integration.Caching;

public class ServiceCachedRepositoryTests
{
    private readonly Mock<IServiceRepository>           /**/ _innerRepo;
    private readonly IMemoryCache                       /**/ _memoryCache;
    private readonly ServiceCachedRepository<Service>   /**/ _cachedRepo;

    public ServiceCachedRepositoryTests()
    {
        _innerRepo      /**/ = new Mock<IServiceRepository>();
        _memoryCache    /**/ = new MemoryCache(new MemoryCacheOptions());
        _cachedRepo     /**/ = new ServiceCachedRepository<Service>(_innerRepo.Object, _memoryCache);
    }

    [Fact]
    public async Task ListAllActivesAsync_CachesResult()
    {
        var service = new Service { Id = Guid.NewGuid(), Date = DateTime.UtcNow, CarId = Guid.NewGuid(), IsActive = true };
        var list = new List<Service> { service };

        _innerRepo.Setup(r => r.ListAllActivesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result1 = await _cachedRepo.ListAllActivesAsync();
        var result2 = await _cachedRepo.ListAllActivesAsync();

        Assert.Single(result1);
        Assert.Equal(result1[0].Id, result2[0].Id);
        _innerRepo.Verify(r => r.ListAllActivesAsync(It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"{typeof(Service).Name}_AllActives";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task GetActiveByIdAsync_CachesResult()
    {
        var service = new Service { Id = Guid.NewGuid(), CarId = Guid.NewGuid(), Date = DateTime.UtcNow };

        _innerRepo.Setup(r => r.GetActiveByIdAsync(service.Id, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(service);

        var result1 = await _cachedRepo.GetActiveByIdAsync(service.Id);
        var result2 = await _cachedRepo.GetActiveByIdAsync(service.Id);

        Assert.NotNull(result1);
        Assert.Equal(result1!.Id, result2!.Id);

        _innerRepo.Verify(r => r.GetActiveByIdAsync(service.Id, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"{typeof(Service).Name}_ById_{service.Id}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task GetScheduledServicesAsync_CachesResult()
    {
        var from = DateTime.Today;
        var to = from.AddDays(7);
        var tuple = new List<(string, string, DateTime)>
        {
            ("Civic", "Sedan", from.AddDays(1))
        };

        _innerRepo.Setup(r => r.GetScheduledServicesAsync(from, to, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(tuple);

        var result1 = await _cachedRepo.GetScheduledServicesAsync(from, to);
        var result2 = await _cachedRepo.GetScheduledServicesAsync(from, to);

        Assert.Single(result1);
        Assert.Equal(result1[0].Model, result2[0].Model);

        _innerRepo.Verify(r => r.GetScheduledServicesAsync(from, to, It.IsAny<CancellationToken>()), Times.Once);

        var keyRange = $"{from:yyyyMMdd}_{to:yyyyMMdd}";
        var cacheKey = $"{typeof(Service).Name}_Scheduled_{keyRange}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task FindActivesByCarAndDateAsync_CachesResult()
    {
        var carId = Guid.NewGuid();
        var date = DateTime.Today;
        var services = new List<Service> { new Service { Id = Guid.NewGuid(), CarId = carId, Date = date } };

        _innerRepo.Setup(r => r.FindActivesByCarAndDateAsync(carId, date, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(services);

        var result1 = await _cachedRepo.FindActivesByCarAndDateAsync(carId, date);
        var result2 = await _cachedRepo.FindActivesByCarAndDateAsync(carId, date);

        Assert.Single(result1);
        Assert.Equal(result1[0].Id, result2[0].Id);

        _innerRepo.Verify(r => r.FindActivesByCarAndDateAsync(carId, date, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"{typeof(Service).Name}_Car_{carId}_Date_{date:yyyyMMdd}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task AddAsync_DelegatesAndInvalidatesCache()
    {
        var service = new Service { Id = Guid.NewGuid() };

        _innerRepo.Setup(r => r.AddAsync(service, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _cachedRepo.AddAsync(service);

        _innerRepo.Verify(r => r.AddAsync(service, It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(_memoryCache.TryGetValue($"{typeof(Service).Name}_AllActives", out _));
        Assert.False(_memoryCache.TryGetValue($"{typeof(Service).Name}_ById_{service.Id}", out _));
    }

    [Fact]
    public async Task UpdateAsync_DelegatesAndInvalidatesCache()
    {
        var service = new Service { Id = Guid.NewGuid() };

        _innerRepo.Setup(r => r.UpdateAsync(service, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _cachedRepo.UpdateAsync(service);

        _innerRepo.Verify(r => r.UpdateAsync(service, It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(_memoryCache.TryGetValue($"{typeof(Service).Name}_AllActives", out _));
        Assert.False(_memoryCache.TryGetValue($"{typeof(Service).Name}_ById_{service.Id}", out _));
    }

    [Fact]
    public async Task DeleteAsync_DelegatesAndInvalidatesCache()
    {
        var service = new Service { Id = Guid.NewGuid() };

        _innerRepo.Setup(r => r.DeleteAsync(service, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _cachedRepo.DeleteAsync(service);

        _innerRepo.Verify(r => r.DeleteAsync(service, It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(_memoryCache.TryGetValue($"{typeof(Service).Name}_AllActives", out _));
        Assert.False(_memoryCache.TryGetValue($"{typeof(Service).Name}_ById_{service.Id}", out _));
    }
}
