/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Caching;

using Microsoft.Extensions.Caching.Memory;

using Moq;

namespace CarRental.Tests.Integration.Caching;

public class CarCachedRepositoryTests
{
    private readonly Mock<ICarRepository>       /**/ _innerRepo;
    private readonly IMemoryCache               /**/ _memoryCache;
    private readonly CarCachedRepository<Car>   /**/ _cachedRepo;

    public CarCachedRepositoryTests()
    {
        _innerRepo      /**/ = new Mock<ICarRepository>();
        _memoryCache    /**/ = new MemoryCache(new MemoryCacheOptions());
        _cachedRepo     /**/ = new CarCachedRepository<Car>(_innerRepo.Object, _memoryCache);
    }

    [Fact]
    public async Task ShouldReturnCachedItemsAfterFirstCall()
    {
        // Arrange
        var car = new Car
        {
            Id          /**/ = Guid.NewGuid(),
            Model       /**/ = "Civic",
            Type        /**/ = "Sedan",
            IsActive    /**/ = true,
            Version     /**/ = 1
        };

        var carList = new List<Car> { car };

        _innerRepo.Setup(r => r.ListAllActivesAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(carList);

        // Act - primera llamada (cache vacío, invoca innerRepo)
        var firstCall = await _cachedRepo.ListAllActivesAsync();

        // Act - segunda llamada (debería usar cache, no llamar innerRepo)
        var secondCall = await _cachedRepo.ListAllActivesAsync();

        // Assert
        Assert.Single(firstCall);
        Assert.Single(secondCall);
        Assert.Equal(firstCall[0].Id, secondCall[0].Id);

        // Verifica que solo se llamó 1 vez al método ListAllActivesAsync del inner repo
        _innerRepo.Verify(r => r.ListAllActivesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Verifica que el cache tiene la clave esperada
        var cacheKey = $"{typeof(Car).Name}_AllActives";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out var cached));
        Assert.NotNull(cached);
    }
    [Fact]
    public async Task ListAllActivesAsync_CachesResult()
    {
        var car  /**/ = new Car { Id = Guid.NewGuid(), Model = "Civic", Type = "Sedan", IsActive = true };
        var list /**/ = new List<Car> { car };

        _innerRepo.Setup(r => r.ListAllActivesAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(list);

        var firstCall   /**/ = await _cachedRepo.ListAllActivesAsync();
        var secondCall  /**/ = await _cachedRepo.ListAllActivesAsync();

        Assert.Single(firstCall);
        Assert.Single(secondCall);
        Assert.Equal(firstCall[0].Id, secondCall[0].Id);

        _innerRepo.Verify(r => r.ListAllActivesAsync(It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"{typeof(Car).Name}_AllActives";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out var cached));
    }

    [Fact]
    public async Task FindByModelAndTypeAsync_CachesResult()
    {
        var model   /**/ = "Civic";
        var type    /**/ = "Sedan";
        var car     /**/ = new Car { Id = Guid.NewGuid(), Model = model, Type = type, IsActive = true };
        var cars    /**/ = new List<Car> { car };

        _innerRepo.Setup(r => r.FindByModelAndTypeAsync(model, type, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(cars);

        var result1 = await _cachedRepo.FindByModelAndTypeAsync(model, type);
        var result2 = await _cachedRepo.FindByModelAndTypeAsync(model, type);

        Assert.Single(result1);
        Assert.Single(result2);
        Assert.Equal(result1[0].Id, result2[0].Id);

        _innerRepo.Verify(r => r.FindByModelAndTypeAsync(model, type, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"{typeof(Car).Name}_Model_{model}_Type_{type}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out var cached));
    }

    [Fact]
    public async Task GetActiveByIdAsync_CachesResult()
    {
        var car = new Car { Id = Guid.NewGuid(), Model = "Focus", Type = "Hatchback", IsActive = true };

        _innerRepo.Setup(r => r.GetActiveByIdAsync(car.Id, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(car);

        var result1 = await _cachedRepo.GetActiveByIdAsync(car.Id);
        var result2 = await _cachedRepo.GetActiveByIdAsync(car.Id);

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1!.Id, result2!.Id);

        _innerRepo.Verify(r => r.GetActiveByIdAsync(car.Id, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"{typeof(Car).Name}_ById_{car.Id}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out var cached));
    }

    [Fact]
    public async Task IsAvailableAsync_DelegatesToInnerRepo()
    {
        var carId   /**/ = Guid.NewGuid();
        var start   /**/ = DateTime.UtcNow;
        var end     /**/ = start.AddDays(1);

        _innerRepo.Setup(r => r.IsAvailableAsync(carId, start, end, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

        var available = await _cachedRepo.IsAvailableAsync(carId, start, end);

        Assert.True(available);
        _innerRepo.Verify(r => r.IsAvailableAsync(carId, start, end, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_UpdatesInnerRepoAndInvalidatesCache()
    {
        var car = new Car { Id = Guid.NewGuid() };

        _innerRepo.Setup(r => r.AddAsync(car, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _cachedRepo.AddAsync(car);

        _innerRepo.Verify(r => r.AddAsync(car, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKeyAll     /**/ = $"{typeof(Car).Name}_AllActives";
        var cacheKeyById    /**/ = $"{typeof(Car).Name}_ById_{car.Id}";

        Assert.False(_memoryCache.TryGetValue(cacheKeyAll, out _));
        Assert.False(_memoryCache.TryGetValue(cacheKeyById, out _));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesInnerRepoAndInvalidatesCache()
    {
        var car = new Car { Id = Guid.NewGuid() };

        _innerRepo.Setup(r => r.UpdateAsync(car, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _cachedRepo.UpdateAsync(car);

        _innerRepo.Verify(r => r.UpdateAsync(car, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKeyAll     /**/ = $"{typeof(Car).Name}_AllActives";
        var cacheKeyById    /**/ = $"{typeof(Car).Name}_ById_{car.Id}";

        Assert.False(_memoryCache.TryGetValue(cacheKeyAll, out _));
        Assert.False(_memoryCache.TryGetValue(cacheKeyById, out _));
    }

    [Fact]
    public async Task DeleteAsync_DeletesFromInnerRepoAndInvalidatesCache()
    {
        var car = new Car { Id = Guid.NewGuid() };

        _innerRepo.Setup(r => r.DeleteAsync(car, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _cachedRepo.DeleteAsync(car);

        _innerRepo.Verify(r => r.DeleteAsync(car, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKeyAll     /**/ = $"{typeof(Car).Name}_AllActives";
        var cacheKeyById    /**/ = $"{typeof(Car).Name}_ById_{car.Id}";

        Assert.False(_memoryCache.TryGetValue(cacheKeyAll, out _));
        Assert.False(_memoryCache.TryGetValue(cacheKeyById, out _));
    }
}
