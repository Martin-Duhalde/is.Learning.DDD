using CarRental.Infrastructure.Auth;
using CarRental.Infrastructure.Databases;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CarRental.Tests.Functional.Common;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    /// <summary>
    /// Overriding CreateHost to avoid creating a separate ServiceProvider per this thread:
    /// https://github.com/dotnet-architecture/eShopOnWeb/issues/465
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development"); // will not send real emails
        var host = builder.Build();
        host.Start();

        // Get service provider.
        var serviceProvider = host.Services;

        // Create a scope to obtain a reference to the database
        // context (CarRentalDbContext).
        using (var scope = serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<CarRentalDbContext>();

            var logger = scopedServices
                .GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

            // Reset Sqlite database for each test run
            // If using a real database, you'll likely want to remove this step.
            db.Database.EnsureDeleted();

            // Ensure the database is created.
            db.Database.EnsureCreated();

            try
            {
                // Seed the database.
                AuthSeeder.SeedRolesAsync(scopedServices, logger).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the " +
                                    "database with test messages. Error: {exceptionMessage}", ex.Message);
            }
        }

        return host;
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Elimina el DbContext original registrado por la app
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CarRentalDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Genera una ruta única por test
            var dbPath = $"CarRental_{Guid.NewGuid()}.db";

            // Reemplaza el DbContext con una base SQLite única
            services.AddDbContext<CarRentalDbContext>(options =>
            {
                options.UseSqlite($"Data Source={dbPath}");
            });

            // Fuerza la creación de la base nueva
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
    //protected override void ConfigureWebHost(IWebHostBuilder builder)
    //{
    //    builder
    //        .ConfigureServices(services =>
    //        {
    //            // Configure test dependencies here

    //            //// Remove the app's ApplicationDbContext registration.
    //            //var descriptor = services.SingleOrDefault(
    //            //d => d.ServiceType ==
    //            //    typeof(DbContextOptions<AppDbContext>));

    //            //if (descriptor != null)
    //            //{
    //            //  services.Remove(descriptor);
    //            //}

    //            //// This should be set for each individual test run
    //            //string inMemoryCollectionName = Guid.NewGuid().ToString();

    //            //// Add ApplicationDbContext using an in-memory database for testing.
    //            //services.AddDbContext<AppDbContext>(options =>
    //            //{
    //            //  options.UseInMemoryDatabase(inMemoryCollectionName);
    //            //});
    //        });
    //}
}
