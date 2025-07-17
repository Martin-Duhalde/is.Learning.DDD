/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

using System.Reflection;

namespace CarRental.Tests.Integration.Databases;

public class CarRentalDbContextTests
{
    private static CarRentalDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CarRentalDbContext(options);
    }

    private DbContextOptions<CarRentalDbContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Can_Create_Context_With_Parameterless_Constructor()
    {
        var context = new CarRentalDbContext();
        Assert.NotNull(context);
    }

    [Fact]
    public void Can_Create_Context_With_Options()
    {
        var options = GetInMemoryOptions();
        var context = new CarRentalDbContext(options);
        Assert.NotNull(context);
    }

    [Fact]
    public void Can_Access_DbSets()
    {
        var options = GetInMemoryOptions();
        using var context = new CarRentalDbContext(options);

        Assert.NotNull(context.Cars);
        Assert.NotNull(context.Customers);
        Assert.NotNull(context.Rentals);
        Assert.NotNull(context.Services);
    }

    [Fact]
    public async Task Should_Save_Service_With_Car()
    {
        using var context = CreateContext();

        var car = new Car { Model = "Toyota", Type = "SUV" };
        var service = new Service { Car = car, Date = DateTime.UtcNow };

        context.Services.Add(service);
        await context.SaveChangesAsync();

        var saved = await context.Services.Include(s => s.Car).FirstAsync();
        Assert.Equal("Toyota", saved.Car!.Model);
    }

    [Fact]
    public async Task Should_Save_Rental_With_Status_Cancelled()
    {
        using var context = CreateContext();

        var customer = new Customer { FullName = "Jane Smith", Address = "Calle 123", UserId = "user-1" };
        var car = new Car { Model = "Ford", Type = "Sedan" };

        context.AddRange(car, customer);
        await context.SaveChangesAsync();

        var rental = new Rental
        {
            CarId = car.Id,
            CustomerId = customer.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            RentalStatus = RentalStatus.Cancelled,
            CancelledAt = DateTime.UtcNow
        };

        context.Rentals.Add(rental);
        await context.SaveChangesAsync();

        var retrieved = await context.Rentals.FirstAsync();
        Assert.Equal(RentalStatus.Cancelled, retrieved.RentalStatus);
        Assert.NotNull(retrieved.CancelledAt);
    }

    [Fact]
    public async Task Should_Persist_IsActive_LogicalDelete()
    {
        using var context = CreateContext();

        var customer = new Customer { FullName = "Inactive Customer", IsActive = false, UserId = "user-x" };
        var car = new Car { Model = "Chevy", Type = "Hatch", IsActive = false };
        var service = new Service { Car = car, Date = DateTime.UtcNow, IsActive = false };

        context.AddRange(customer, car, service);
        await context.SaveChangesAsync();

        var savedCustomer = await context.Customers.FirstAsync();
        var savedCar = await context.Cars.FirstAsync();
        var savedService = await context.Services.FirstAsync();

        Assert.False(savedCustomer.IsActive);
        Assert.False(savedCar.IsActive);
        Assert.False(savedService.IsActive);
    }

    [Fact]
    public async Task Should_Include_Multiple_Services_For_Car()
    {
        using var context = CreateContext();

        var car = new Car { Model = "Jeep", Type = "4x4" };
        var services = new[]
        {
            new Service { Car = car, Date = DateTime.UtcNow },
            new Service { Car = car, Date = DateTime.UtcNow.AddDays(1) }
        };

        context.AddRange(services);
        await context.SaveChangesAsync();

        var savedCar = await context.Cars.Include(c => c.Services).FirstAsync();
        Assert.Equal(2, savedCar.Services.Count);
    }

    [Fact]
    public async Task Should_Persist_Version_Field()
    {
        using var context = CreateContext();

        var customer = new Customer { FullName = "Versioned", Version = 3, UserId = "v123" };
        var car = new Car { Model = "Tesla", Type = "Electric", Version = 5 };

        context.AddRange(customer, car);
        await context.SaveChangesAsync();

        var savedCustomer = await context.Customers.FirstAsync();
        var savedCar = await context.Cars.FirstAsync();

        Assert.Equal(3, savedCustomer.Version);
        Assert.Equal(5, savedCar.Version);
    }

    private class TestableCarRentalDbContext : CarRentalDbContext
    {
        public TestableCarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : base(options) { }

        // Exponer el método privado para testeo
        public void CallOnModelCreatingSQLServer(ModelBuilder modelBuilder)
        {
            // Usamos reflection para invocar el método privado
            var method = typeof(CarRentalDbContext)
                .GetMethod("OnModelCreatingSQLServer", BindingFlags.Instance | BindingFlags.NonPublic);
            method!.Invoke(this, new object[] { modelBuilder });
        }
        public override bool IsOnSqlServer()
        {
            return base.IsOnSqlServer();
        }
    }

    [Fact]
    public void OnModelCreatingSQLServer_Configures_Entities_UniqueIdentifier_And_ValueGeneratedOnAdd()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        var context = new TestableCarRentalDbContext(options);

        var modelBuilder = new ModelBuilder(new ConventionSet());

        context.CallOnModelCreatingSQLServer(modelBuilder);

        var entities = new[] { typeof(Car), typeof(Customer), typeof(Rental), typeof(Service) };

        context.IsOnSqlServer();

        foreach (var entityType in entities)
        {
            var entity = modelBuilder.Model.FindEntityType(entityType);
            Assert.NotNull(entity);

            var idProperty = entity!.FindProperty(nameof(IEntity.Id));
            Assert.NotNull(idProperty);

            Assert.Equal("uniqueidentifier", idProperty!.GetColumnType());
            Assert.Equal(ValueGenerated.OnAdd, idProperty.ValueGenerated);
        }
    }

}

