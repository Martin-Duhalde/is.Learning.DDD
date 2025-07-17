/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Caching;

using Microsoft.Extensions.Caching.Memory;

using Moq;

namespace CarRental.Tests.Integration.Caching;

public class RentalCachedRepositoryTests
{
    private readonly Mock<IRentalRepository>          /**/ _innerRepo;
    private readonly IMemoryCache                     /**/ _memoryCache;
    private readonly RentalCachedRepository<Rental>   /**/ _cachedRepo;

    public RentalCachedRepositoryTests()
    {
        _innerRepo      /**/ = new Mock<IRentalRepository>();
        _memoryCache    /**/ = new MemoryCache(new MemoryCacheOptions());
        _cachedRepo     /**/ = new RentalCachedRepository<Rental>(_innerRepo.Object, _memoryCache);
    }

    [Fact]
    public async Task ListAllActivesAsync_CachesResult()
    {
        var rental      /**/ = new Rental { Id = Guid.NewGuid(), IsActive = true };
        var list        /**/ = new List<Rental> { rental };

        _innerRepo.Setup(r => r.ListAllActivesAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(list);

        var firstCall   /**/ = await _cachedRepo.ListAllActivesAsync();
        var secondCall  /**/ = await _cachedRepo.ListAllActivesAsync();

        Assert.Single(firstCall);
        Assert.Single(secondCall);
        Assert.Equal(firstCall[0].Id, secondCall[0].Id);

        _innerRepo.Verify(r => r.ListAllActivesAsync(It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"{typeof(Rental).Name}_AllActives";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out var cached));
    }

    [Fact]
    public async Task GetActiveByIdAsync_CachesResult()
    {
        var rental = new Rental { Id = Guid.NewGuid(), IsActive = true };

        _innerRepo.Setup(r => r.GetActiveByIdAsync(rental.Id, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(rental);

        var result1 = await _cachedRepo.GetActiveByIdAsync(rental.Id);
        var result2 = await _cachedRepo.GetActiveByIdAsync(rental.Id);

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1!.Id, result2!.Id);

        _innerRepo.Verify(r => r.GetActiveByIdAsync(rental.Id, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"{typeof(Rental).Name}_ById_{rental.Id}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out var cached));
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_CachesResult()
    {
        var rental = new Rental { Id = Guid.NewGuid(), IsActive = true };

        _innerRepo.Setup(r => r.GetByIdWithDetailsAsync(rental.Id, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(rental);

        var result1 = await _cachedRepo.GetByIdWithDetailsAsync(rental.Id);
        var result2 = await _cachedRepo.GetByIdWithDetailsAsync(rental.Id);

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1!.Id, result2!.Id);

        _innerRepo.Verify(r => r.GetByIdWithDetailsAsync(rental.Id, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"{typeof(Rental).Name}_WithDetails_{rental.Id}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out var cached));
    }

    [Fact]
    public async Task ExistsAsync_DelegatesToInnerRepo()
    {
        var carId  /**/ = Guid.NewGuid();
        var start  /**/ = DateTime.UtcNow;
        var end    /**/ = start.AddDays(3);

        _innerRepo.Setup(r => r.ExistsAsync(carId, start, end, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

        var exists = await _cachedRepo.ExistsAsync(carId, start, end);

        Assert.True(exists);
        _innerRepo.Verify(r => r.ExistsAsync(carId, start, end, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_UpdatesInnerRepoAndInvalidatesCache()
    {
        var rental = new Rental { Id = Guid.NewGuid() };

        _innerRepo.Setup(r => r.AddAsync(rental, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _cachedRepo.AddAsync(rental);

        _innerRepo.Verify(r => r.AddAsync(rental, It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(_memoryCache.TryGetValue($"{typeof(Rental).Name}_AllActives", out _));
        Assert.False(_memoryCache.TryGetValue($"{typeof(Rental).Name}_ById_{rental.Id}", out _));
        Assert.False(_memoryCache.TryGetValue($"{typeof(Rental).Name}_WithDetails_{rental.Id}", out _));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesInnerRepoAndInvalidatesCache()
    {
        var rental = new Rental { Id = Guid.NewGuid() };

        _innerRepo.Setup(r => r.UpdateAsync(rental, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _cachedRepo.UpdateAsync(rental);

        _innerRepo.Verify(r => r.UpdateAsync(rental, It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(_memoryCache.TryGetValue($"{typeof(Rental).Name}_AllActives", out _));
        Assert.False(_memoryCache.TryGetValue($"{typeof(Rental).Name}_ById_{rental.Id}", out _));
        Assert.False(_memoryCache.TryGetValue($"{typeof(Rental).Name}_WithDetails_{rental.Id}", out _));
    }

    [Fact]
    public async Task DeleteAsync_DeletesFromInnerRepoAndInvalidatesCache()
    {
        var rental = new Rental { Id = Guid.NewGuid() };

        _innerRepo.Setup(r => r.DeleteAsync(rental, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        await _cachedRepo.DeleteAsync(rental);

        _innerRepo.Verify(r => r.DeleteAsync(rental, It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(_memoryCache.TryGetValue($"{typeof(Rental).Name}_AllActives", out _));
        Assert.False(_memoryCache.TryGetValue($"{typeof(Rental).Name}_ById_{rental.Id}", out _));
        Assert.False(_memoryCache.TryGetValue($"{typeof(Rental).Name}_WithDetails_{rental.Id}", out _));
    }


    [Fact]
    public async Task GetRentalsByCustomerIdAsync_CachesResult()
    {
        var customerId = Guid.NewGuid();
        var rentals = new List<Rental> { new Rental { Id = Guid.NewGuid(), CustomerId = customerId } };

        _innerRepo.Setup(r => r.GetRentalsByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(rentals);

        var result1 = await _cachedRepo.GetRentalsByCustomerIdAsync(customerId);
        var result2 = await _cachedRepo.GetRentalsByCustomerIdAsync(customerId);

        Assert.Single(result1);
        Assert.Equal(result1[0].Id, result2[0].Id);
        _innerRepo.Verify(r => r.GetRentalsByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"Rental_Customer_{customerId}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task ListActivesBetweenDatesAsync_CachesResult()
    {
        var from = DateTime.Today;
        var to = from.AddDays(3);
        var rentals = new List<Rental> { new Rental { Id = Guid.NewGuid() } };

        _innerRepo.Setup(r => r.ListActivesBetweenDatesAsync(from, to, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(rentals);

        var result1 = await _cachedRepo.ListActivesBetweenDatesAsync(from, to);
        var result2 = await _cachedRepo.ListActivesBetweenDatesAsync(from, to);

        Assert.Single(result1);
        Assert.Equal(result1[0].Id, result2[0].Id);
        _innerRepo.Verify(r => r.ListActivesBetweenDatesAsync(from, to, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"Rental_ActivesBetween_{from:yyyyMMdd}_{to:yyyyMMdd}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task ListLast7DaysAsync_CachesResult()
    {
        var rentals = new List<Rental> { new Rental { Id = Guid.NewGuid() } };

        _innerRepo.Setup(r => r.ListLast7DaysAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(rentals);

        var result1 = await _cachedRepo.ListLast7DaysAsync();
        var result2 = await _cachedRepo.ListLast7DaysAsync();

        Assert.Single(result1);
        Assert.Equal(result1[0].Id, result2[0].Id);
        _innerRepo.Verify(r => r.ListLast7DaysAsync(It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = "Rental_Last7Days";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task ListCancelledAsync_CachesResult()
    {
        var from = DateTime.Today;
        var to = from.AddDays(2);
        var rentals = new List<Rental> { new Rental { Id = Guid.NewGuid() } };

        _innerRepo.Setup(r => r.ListCancelledAsync(from, to, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(rentals);

        var result1 = await _cachedRepo.ListCancelledAsync(from, to);
        var result2 = await _cachedRepo.ListCancelledAsync(from, to);

        Assert.Single(result1);
        Assert.Equal(result1[0].Id, result2[0].Id);
        _innerRepo.Verify(r => r.ListCancelledAsync(from, to, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"Rental_Cancelled_{from:yyyyMMdd}_{to:yyyyMMdd}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task CountCancelledAsync_CachesResult()
    {
        var from = DateTime.Today;
        var to = from.AddDays(2);
        var count = 3;

        _innerRepo.Setup(r => r.CountCancelledAsync(from, to, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(count);

        var result1 = await _cachedRepo.CountCancelledAsync(from, to);
        var result2 = await _cachedRepo.CountCancelledAsync(from, to);

        Assert.Equal(count, result1);
        Assert.Equal(result1, result2);
        _innerRepo.Verify(r => r.CountCancelledAsync(from, to, It.IsAny<CancellationToken>()), Times.Once);

        var cacheKey = $"Rental_CountCancelled_{from:yyyyMMdd}_{to:yyyyMMdd}";
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task CancelAsync_DelegatesAndInvalidatesCache()
    {
        var rentalId = Guid.NewGuid();

        _innerRepo.Setup(r => r.CancelAsync(rentalId, It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        // Set dummy values in cache
        _memoryCache.Set("Rental_AllActives", new List<Rental>());
        _memoryCache.Set($"Rental_ById_{rentalId}", new Rental { Id = rentalId });

        await _cachedRepo.CancelAsync(rentalId);

        _innerRepo.Verify(r => r.CancelAsync(rentalId, It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(_memoryCache.TryGetValue("Rental_AllActives", out _));
        Assert.False(_memoryCache.TryGetValue($"Rental_ById_{rentalId}", out _));
    }
}
