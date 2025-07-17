/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;

using FluentAssertions;

namespace CarRental.Tests.Integration.Repositories;

public class EfRepositoryTest
{
    private readonly DbContextOptions<CarRentalDbContext> _dbOptions;

    public EfRepositoryTest()
    {
        _dbOptions = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    /// 🧪 validates: AddAsync initializes Version = 1 and IsActive = true
    [Fact]
    public async Task should_initialize_entity_with_version_and_active_defaults()
    {
        using var context = new CarRentalDbContext(_dbOptions);
        var repository = new EfRepository<Customer>(context);

        var entity = new Customer { FullName = "Test User" };

        await repository.AddAsync(entity);

        var stored = await context.Customers.FirstOrDefaultAsync();

        stored.Should().NotBeNull();
        stored!.Version.Should().Be(1);
        stored.IsActive.Should().BeTrue();
    }

    /// 🧪 validates: DeleteAsync performs soft-delete and increments Version
    [Fact]
    public async Task should_soft_delete_entity_and_increment_version()
    {
        using var context = new CarRentalDbContext(_dbOptions);
        var repository = new EfRepository<Customer>(context);

        var entity = new Customer { FullName = "Delete Test" };
        await repository.AddAsync(entity);

        await repository.DeleteAsync(entity);

        var deleted = await context.Customers.FirstOrDefaultAsync();
        deleted!.IsActive.Should().BeFalse();
        deleted.Version.Should().Be(2);
    }

    /// 🧪 validates: DeleteAsync throws if entity does not exist
    [Fact]
    public async Task should_throw_if_entity_not_found_when_deleting()
    {
        using var context = new CarRentalDbContext(_dbOptions);
        var repository = new EfRepository<Customer>(context);

        var ghost = new Customer { Id = Guid.NewGuid(), FullName = "Ghost" };

        var act = async () => await repository.DeleteAsync(ghost);
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Entity not found");
    }

    /// 🧪 validates: DeleteAsync throws if entity is already inactive
    [Fact]
    public async Task should_throw_if_entity_already_deleted_when_deleting()
    {
        using var context = new CarRentalDbContext(_dbOptions);
        var repository = new EfRepository<Customer>(context);

        var entity = new Customer { FullName = "Deleted One" };
        await repository.AddAsync(entity);
        await repository.DeleteAsync(entity);

        var act = async () => await repository.DeleteAsync(entity);
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Entity already deleted.");
    }

    /// 🧪 validates: UpdateAsync updates fields and increments Version
    [Fact]
    public async Task should_update_entity_and_increment_version()
    {
        using var context = new CarRentalDbContext(_dbOptions);
        var repository = new EfRepository<Customer>(context);

        var entity = new Customer { FullName = "Original" };
        await repository.AddAsync(entity);

        entity.FullName = "Updated";
        await repository.UpdateAsync(entity);

        var updated = await context.Customers.FirstOrDefaultAsync();
        updated!.FullName.Should().Be("Updated");
        updated.Version.Should().Be(2);
    }

    /// 🧪 validates: UpdateAsync throws if version mismatch (concurrency conflict)
    [Fact]
    public async Task should_throw_on_concurrency_conflict_when_updating()
    {
        using var context = new CarRentalDbContext(_dbOptions);
        var repository = new EfRepository<Customer>(context);

        var entity = new Customer { FullName = "Concurrent" };
        await repository.AddAsync(entity);

        entity.Version = 999; // simulate outdated version

        var act = async () => await repository.UpdateAsync(entity);
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Entity was modified by another process. (Concurrency Exception)");
    }

    /// 🧪 validates: ListAllActivesAsync returns only active records
    [Fact]
    public async Task should_return_only_active_entities_from_list_all()
    {
        using var context = new CarRentalDbContext(_dbOptions);
        var repository = new EfRepository<Customer>(context);

        var active = new Customer { FullName = "Active" };
        var inactive = new Customer { FullName = "Inactive", IsActive = false };

        context.Customers.AddRange(active, inactive);
        await context.SaveChangesAsync();

        var result = await repository.ListAllActivesAsync();

        result.Should().ContainSingle()
            .Which.FullName.Should().Be("Active");
    }

    /// 🧪 validates: GetActiveByIdAsync returns entity only if active
    [Fact]
    public async Task should_return_active_entity_by_id_only_if_not_deleted()
    {
        using var context = new CarRentalDbContext(_dbOptions);
        var repository = new EfRepository<Customer>(context);

        var active = new Customer { FullName = "Active" };
        var deleted = new Customer { FullName = "Deleted", IsActive = false };

        context.Customers.AddRange(active, deleted);
        await context.SaveChangesAsync();

        var found = await repository.GetActiveByIdAsync(active.Id);
        found.Should().NotBeNull();

        var notFound = await repository.GetActiveByIdAsync(deleted.Id);
        notFound.Should().BeNull();
    }
}
