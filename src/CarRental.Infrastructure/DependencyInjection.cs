/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Application.Abstractions.Interfaces;
using CarRental.Application.Abstractions.Repositories;
using CarRental.Infrastructure.Auth;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Repositories;
using CarRental.Infrastructure.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<IUserDirectory, UserDirectory>();
        services.AddScoped<ICarAvailabilityReadService, CarAvailabilityReadService>();
        return services;
    }
}
