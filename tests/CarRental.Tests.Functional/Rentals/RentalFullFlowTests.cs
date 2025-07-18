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
public class RentalFullFlowTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient         /**/ _client    /**/ = factory.CreateClient();
    private readonly IServiceProvider   /**/ _services  /**/ = factory.Services;

    [Fact]
    public async Task should_complete_full_rental_flow_successfully()
    {
        Guid carId;

        // --- Insertar un auto válido en la base de datos de pruebas ---
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();

            var car = new Car
            {
                Id      /**/ = Guid.NewGuid(),
                Model   /**/ = "ModelX",
                Type    /**/ = "SUV"
            };

            db.Cars.Add(car);
            await db.SaveChangesAsync();

            carId = car.Id;
        }

        // ---------- 1. Registro ----------
        var registerDto = new RegisterDto
        {
            FullName    /**/ = "Funcional Tester",
            Email       /**/ = $"user{Guid.NewGuid():N}@mail.com",
            Password    /**/ = "StrongPass123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        registerResponse.EnsureSuccessStatusCode();

        // ---------- 2. Login ----------
        var loginDto = new LoginDto
        {
            Email       /**/ = registerDto.Email,
            Password    /**/ = registerDto.Password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();

        Assert.False(string.IsNullOrEmpty(loginResult?.Token));

        // Configuro JWT para las siguientes requests
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // ---------- 3. Verificar endpoint protegido ----------
        var testResponse = await _client.GetAsync("/api/authtest/user");
        testResponse.EnsureSuccessStatusCode();

        var testMessage = await testResponse.Content.ReadAsStringAsync();
        Assert.Contains("🔐", testMessage);

        // ---------- 4. Consultar autos disponibles ----------
        var startDate           /**/ = DateTime.UtcNow.AddDays(1);
        var endDate             /**/ = DateTime.UtcNow.AddDays(2);
        var availabilityUri     /**/ = $"/api/rental/availability?startDate={startDate:O}&endDate={endDate:O}&type=SUV&model=ModelX";

        var availabilityResponse = await _client.GetAsync(availabilityUri);
        availabilityResponse.EnsureSuccessStatusCode();

        var cars = await availabilityResponse.Content.ReadFromJsonAsync<List<CarAvailabilityDto>>();
        
        Assert.NotNull(cars);
        Assert.NotEmpty(cars);

        var carFromApi = cars.First();
        
        Assert.Equal(carId, carFromApi.Id);
        Assert.Equal("SUV", carFromApi.Type);
        Assert.Equal("ModelX", carFromApi.Model);

        // ---------- 5. Obtener UserId desde BD ----------
        string userStringId;
        Guid userId;
        using (var scope = _services.CreateScope())
        {
            var db          /**/ = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            userStringId    /**/ = db.Users.First(u => u.Email == registerDto.Email).Id;
            userId          /**/ = Guid.Parse(userStringId);
        }

        // ---------- 6. Insertar Customer vinculado al usuario ----------
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();

            var customer = new Customer
            {
                Id          /**/ = userId,
                FullName    /**/ = registerDto.FullName,
                Address     /**/ = "123 Fake Street",
                UserId      /**/ = userStringId
            };

            db.Customers.Add(customer);
            await db.SaveChangesAsync();
        }

        // ---------- 7. Crear reserva ----------
        var rentalDto = new CreateRentalRequestDto
        {
            CarId           /**/ = carFromApi.Id,
            StartDate       /**/ = startDate,
            EndDate         /**/ = endDate,
            CustomerId      /**/ = userId
        };

        var createRentalResponse = await _client.PostAsJsonAsync("/api/rental", rentalDto);
        createRentalResponse.EnsureSuccessStatusCode();

        var rentalResponse = await createRentalResponse.Content.ReadFromJsonAsync<CreateRentalResponseDto>();

        Assert.NotNull(rentalResponse);
        Assert.NotEqual(Guid.Empty, rentalResponse.RentalId);
        //var rentalId = await createRentalResponse.Content.ReadFromJsonAsync<CreateRentalResponseDto>();
        //Assert.NotEqual(Guid.Empty, rentalId.RentalId);

        // ---------- 8. Cancelar reserva ----------
        var cancelResponse = await _client.DeleteAsync($"/api/rental/{rentalResponse.RentalId}");
        cancelResponse.EnsureSuccessStatusCode();

        // ---------- 9. Verificar que ya no está disponible la reserva activa para ese rango ----------
        // La lógica de IsAvailableAsync considera sólo reservas activas, por eso esta nueva consulta
        var checkAvailabilityResponse = await _client.GetAsync(availabilityUri);
        checkAvailabilityResponse.EnsureSuccessStatusCode();

        var availableCarsAfterCancel = await checkAvailabilityResponse.Content.ReadFromJsonAsync<List<CarAvailabilityDto>>();
       
        Assert.NotNull(availableCarsAfterCancel);
        Assert.NotEmpty(availableCarsAfterCancel);

        var carAvailableAfterCancel = availableCarsAfterCancel.FirstOrDefault(c => c.Id == carId);
        
        Assert.NotNull(carAvailableAfterCancel); // El coche debe estar disponible ahora que la reserva fue cancelada
    }

}
