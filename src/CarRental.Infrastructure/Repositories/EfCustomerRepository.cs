/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;

namespace CarRental.Infrastructure.Repositories;

public class EfCustomerRepository : EfRepository<Customer>, ICustomerRepository
{
    public EfCustomerRepository(CarRentalDbContext db) : base(db) { }
}
