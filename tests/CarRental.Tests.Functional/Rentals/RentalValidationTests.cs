/// MIT License © 2025 Martín Duhalde + ChatGPT

/// NO NORMALIZADO! Esta clase será eliminada en el futuro.

/// Ver normalización: 
/// 
///     CarApiFlowTests  en  CarRental.Tests.Functional.Cars;

using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Tests.Functional.Common;
using CarRental.UseCases.Auth.Dtos;
using CarRental.UseCases.Rentals.Dtos;

using Microsoft.Extensions.DependencyInjection;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CarRental.Tests.Functional.Rentals;
[Collection("Sequential")]
public class RentalValidationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IServiceProvider _services = factory.Services;

    [Fact]
    public async Task should_pass_validation_with_valid_rental_dto()
    {
        Guid carId;
        Guid userId;
        string userStringId;

        // ---------- 1. Insertar auto válido ----------
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            var car = new Car { Id = Guid.NewGuid(), Type = "SUV", Model = "ModelX" };
            db.Cars.Add(car);
            await db.SaveChangesAsync();
            carId = car.Id;
        }

        // ---------- 2. Registrar usuario ----------
        var registerDto = new RegisterDto
        {
            FullName = "Validator Tester",
            Email = $"user{Guid.NewGuid():N}@mail.com",
            Password = "Valid123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        registerResponse.EnsureSuccessStatusCode();

        // ---------- 3. Login ----------
        var loginDto = new LoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
        Assert.False(string.IsNullOrEmpty(loginResult?.Token));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        // ---------- 4. Obtener UserId ----------
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            userStringId = db.Users.First(u => u.Email == registerDto.Email).Id;
            userId = Guid.Parse(userStringId);
        }

        // ---------- 5. Insertar Customer ----------
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            db.Customers.Add(new Customer
            {
                Id = userId,
                FullName = registerDto.FullName,
                Address = "123 Valid St",
                UserId = userStringId
            });
            await db.SaveChangesAsync();
        }

        // ---------- 6. Crear reserva válida ----------
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(1);

        var validRentalDto = new CreateRentalRequestDto
        {
            CarId = carId,
            CustomerId = userId,
            StartDate = startDate,
            EndDate = endDate
        };

        var response = await _client.PostAsJsonAsync("/api/rental", validRentalDto);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CreateRentalResponseDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.RentalId);
    }


    [Fact]
    public async Task should_fail_validation_with_invalid_rental_dto()
    {
        Guid carId;
        Guid userId;
        string userStringId;

        // ---------- 1. Insertar auto válido ----------
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            var car = new Car { Id = Guid.NewGuid(), Type = "SUV", Model = "ModelX" };
            db.Cars.Add(car);
            await db.SaveChangesAsync();
            carId = car.Id;
        }

        // ---------- 2. Registrar usuario ----------
        var registerDto = new RegisterDto
        {
            FullName = "Validator Tester",
            Email = $"user{Guid.NewGuid():N}@mail.com",
            Password = "Valid123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        registerResponse.EnsureSuccessStatusCode();

        // ---------- 3. Login ----------
        var loginDto = new LoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
        Assert.False(string.IsNullOrEmpty(loginResult?.Token));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        // ---------- 4. Obtener UserId desde BD ----------
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            userStringId = db.Users.First(u => u.Email == registerDto.Email).Id;
            userId = Guid.Parse(userStringId);
        }

        // ---------- 5. Insertar Customer ----------
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            db.Customers.Add(new Customer
            {
                Id = userId,
                FullName = registerDto.FullName,
                Address = "123 Fake St",
                UserId = userStringId
            });
            await db.SaveChangesAsync();
        }

        // ---------- 6. Enviar DTO inválido ----------
        var invalidRentalDto = new CreateRentalRequestDto
        {
            CarId = Guid.Empty, // ❌ no válido
            CustomerId = Guid.Empty, // ❌ no válido
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(1) // ❌ start > end
        };

        var response = await _client.PostAsJsonAsync("/api/rental", invalidRentalDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();

        Assert.NotNull(errors);
        Assert.True(errors.ContainsKey(nameof(CreateRentalRequestDto.CarId)));
        Assert.True(errors.ContainsKey(nameof(CreateRentalRequestDto.CustomerId)));
        Assert.True(errors.ContainsKey(nameof(CreateRentalRequestDto.StartDate)));
    }
}