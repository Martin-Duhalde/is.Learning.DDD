/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Core.Interfaces;
using CarRental.Core.Repositories;
using CarRental.Infrastructure.Auth;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;

using Microsoft.AspNetCore.Identity;

namespace CarRental.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        /// DbContext, EmailSender, AuthService, etc...

        /// Repositories
        services.AddScoped<IServiceRepository   /**/ , EfServiceRepository  /**/ >();
        services.AddScoped<ICarRepository       /**/ , EfCarRepository      /**/ >();
        services.AddScoped<ICustomerRepository  /**/ , EfCustomerRepository /**/ >();
        services.AddScoped<IRentalRepository    /**/ , EfRentalRepository   /**/ >();

        /// 💾 Identity
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<CarRentalDbContext>()
            .AddDefaultTokenProviders();

        /// 💾 AuthService
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}