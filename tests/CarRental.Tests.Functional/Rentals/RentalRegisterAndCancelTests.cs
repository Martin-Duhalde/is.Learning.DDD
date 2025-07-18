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

using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CarRental.Tests.Functional.Rentals;

[Collection("Sequential")]
public class RentalRegisterAndCancelTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IServiceProvider _services = factory.Services;

    [Fact]
    public async Task should_register_and_cancel_rental()
    {
        Guid carId;

        // --- 1. Insertar un auto válido en la base de datos ---
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();

            var car = new Car
            {
                Id = Guid.NewGuid(),
                Type = "SUV",
                Model = "ModelX"
            };

            db.Cars.Add(car);
            await db.SaveChangesAsync();

            carId = car.Id;
        }

        // --- 2. Registrar usuario ---
        var registerDto = new RegisterDto
        {
            FullName = "Functional Tester",
            Email = $"user{Guid.NewGuid():N}@mail.com",
            Password = "StrongPass123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        registerResponse.EnsureSuccessStatusCode();

        // --- 3. Login ---
        var loginDto = new LoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
        Assert.False(string.IsNullOrEmpty(loginResult?.Token));

        // --- 4. Obtener UserId desde BD ---
        string userStringId;
        Guid userId;
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            userStringId = db.Users.First(u => u.Email == registerDto.Email).Id;
            userId = Guid.Parse(userStringId);
        }

        // --- 5. Insertar Customer vinculado al usuario ---
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();

            var customer = new Customer
            {
                Id = userId,
                FullName = registerDto.FullName,
                Address = "123 Fake Street",
                UserId = userStringId
            };

            db.Customers.Add(customer);
            await db.SaveChangesAsync();
        }

        // --- 6. Configurar JWT en cliente ---
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        // --- 7. Consultar disponibilidad ---
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(1);
        var availabilityUri = $"/api/rental/availability?startDate={startDate:O}&endDate={endDate:O}&type=SUV&model=ModelX";

        var availabilityResponse = await _client.GetAsync(availabilityUri);
        availabilityResponse.EnsureSuccessStatusCode();

        var cars = await availabilityResponse.Content.ReadFromJsonAsync<List<CarAvailabilityDto>>();
        Assert.NotNull(cars);
        Assert.NotEmpty(cars);

        var availableCar = cars.First();
        Assert.Equal(carId, availableCar.Id);

        // --- 8. Crear reserva ---
        var rentalDto = new CreateRentalRequestDto
        {
            CarId = availableCar.Id,
            StartDate = startDate,
            EndDate = endDate,
            CustomerId = userId
        };

        var createRentalResponse = await _client.PostAsJsonAsync("/api/rental", rentalDto);
        createRentalResponse.EnsureSuccessStatusCode();

        var rentalResponse = await createRentalResponse.Content.ReadFromJsonAsync<CreateRentalResponseDto>();

        Assert.NotNull(rentalResponse);
        Assert.NotEqual(Guid.Empty, rentalResponse.RentalId);

        // --- 9. Cancelar reserva ---
        var cancelResponse = await _client.DeleteAsync($"/api/rental/{rentalResponse.RentalId}");
        cancelResponse.EnsureSuccessStatusCode();


        // --- 10. Verificar que la reserva ya no está activa ---
        // Volvemos a consultar la disponibilidad para el mismo rango de fechas
        var availabilityAfterCancelResponse = await _client.GetAsync(availabilityUri);
        availabilityAfterCancelResponse.EnsureSuccessStatusCode();

        var carsAfterCancel = await availabilityAfterCancelResponse.Content.ReadFromJsonAsync<List<CarAvailabilityDto>>();
        Assert.NotNull(carsAfterCancel);
        Assert.NotEmpty(carsAfterCancel);

        // El auto debe estar disponible después de cancelar la reserva
        var availableCarAfterCancel = carsAfterCancel.FirstOrDefault(c => c.Id == carId);
        Assert.NotNull(availableCarAfterCancel);
    }


}
