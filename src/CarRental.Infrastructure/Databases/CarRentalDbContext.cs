/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Auth;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Databases;

public class CarRentalDbContext : IdentityDbContext<ApplicationUser> //: DbContext
{
    public CarRentalDbContext() { }
    public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options)
        : base(options) { }

    public DbSet<Customer>  /**/ Customers      /**/ => Set<Customer>();
    public DbSet<Car>       /**/ Cars           /**/ => Set<Car>();
    public DbSet<Rental>    /**/ Rentals        /**/ => Set<Rental>();
    public DbSet<Service>   /**/ Services       /**/ => Set<Service>();

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (IsOnSqlServer())
        {
            OnModelCreatingSQLServer(modelBuilder);
        }

        modelBuilder.Entity<Car>()
            .HasMany(c => c.Services)
            .WithOne(s => s.Car)
            .HasForeignKey(s => s.CarId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Car)
            .WithMany()
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
    public virtual bool IsOnSqlServer() /// For test enviroment
    {
        return Database.IsSqlServer();
    }

    private void OnModelCreatingSQLServer(ModelBuilder modelBuilder)
    {
        // GUID debe ser tratado como uniqueidentifier en SQL Server
        modelBuilder.Entity<Car>(entity =>
        {
            entity.Property(e => e.Id)
                  .HasColumnType("uniqueidentifier") // solo necesario para SQL Server
                  .ValueGeneratedOnAdd();            // autogenerado
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.Id)
                  .HasColumnType("uniqueidentifier")
                  .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.Property(e => e.Id)
                  .HasColumnType("uniqueidentifier")
                  .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.Id)
                  .HasColumnType("uniqueidentifier")
                  .ValueGeneratedOnAdd();
        });

    }
}