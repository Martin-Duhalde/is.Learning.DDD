/// MIT License © 2025 Martín Duhalde + ChatGPT

/// NO NORMALIZADO! Esta clase será eliminada en el futuro.

/// Ver normalización: 
/// 
///     CarApiFlowTests  en  CarRental.Tests.Functional.Cars;


using CarRental.Domain.Entities;
using CarRental.Infrastructure.Databases;
using CarRental.Tests.Functional.Common;

/// CarRentalDbContext
using CarRental.UseCases.Auth.Dtos;
using CarRental.UseCases.Rentals.Dtos;

using Microsoft.Extensions.DependencyInjection;

using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CarRental.Tests.Functional.Rentals;

[Collection("Sequential")]
public class RentalRebookFlowTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient         /**/ _client    /**/ = factory.CreateClient();
    private readonly IServiceProvider   /**/ _services  /**/ = factory.Services;

    [Fact]
    public async Task should_cancel_and_rebook_same_rental_period()
    {
        Guid carId;

        // --- Insertar auto válido ---
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            var car = new Car
            {
                Id          /**/ = Guid.NewGuid(),
                Type        /**/ = "SUV",
                Model       /**/ = "ModelX"
            };
            db.Cars.Add(car);
            await db.SaveChangesAsync();
            carId = car.Id;
        }

        // --- Registrar usuario ---
        var registerDto = new RegisterDto
        {
            FullName        /**/ = "Functional Tester",
            Email           /**/ = $"user{Guid.NewGuid():N}@mail.com",
            Password        /**/ = "StrongPass123!"
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        registerResponse.EnsureSuccessStatusCode();

        // --- Login ---
        var loginDto = new LoginDto
        {
            Email           /**/ = registerDto.Email,
            Password        /**/ = registerDto.Password
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();

        Assert.False(string.IsNullOrEmpty(loginResult?.Token));

        // --- Obtener UserId ---
        string userStringId;
        Guid userId;
        using (var scope = _services.CreateScope())
        {
            var db          /**/ = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
            userStringId    /**/ = db.Users.First(u => u.Email == registerDto.Email).Id;
            userId          /**/ = Guid.Parse(userStringId);
        }

        // --- Insertar Customer ---
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

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        // --- Consulta disponibilidad inicial ---
        var startDate       /**/ = DateTime.UtcNow.AddDays(1);
        var endDate         /**/ = startDate.AddDays(1);
        var availabilityUri /**/ = $"/api/rental/availability?startDate={startDate:O}&endDate={endDate:O}&type=SUV&model=ModelX";

        var availabilityResponse = await _client.GetAsync(availabilityUri);
        availabilityResponse.EnsureSuccessStatusCode();

        var cars = await availabilityResponse.Content.ReadFromJsonAsync<List<CarAvailabilityDto>>();

        Assert.NotNull(cars);
        Assert.NotEmpty(cars);

        var availableCar = cars.First();

        Assert.Equal(carId, availableCar.Id);

        // --- Crear primera reserva ---
        var rentalDto = new CreateRentalRequestDto
        {
            CarId       /**/ = availableCar.Id,
            StartDate   /**/ = startDate,
            EndDate     /**/ = endDate,
            CustomerId  /**/ = userId
        };
        var createRentalResponse = await _client.PostAsJsonAsync("/api/rental", rentalDto);
        createRentalResponse.EnsureSuccessStatusCode();

        var firstRentalResponse = await createRentalResponse.Content.ReadFromJsonAsync<CreateRentalResponseDto>();

        Assert.NotNull(firstRentalResponse);

        Assert.NotEqual(Guid.Empty, firstRentalResponse.RentalId);

        // --- Cancelar reserva ---
        var cancelResponse = await _client.DeleteAsync($"/api/rental/{firstRentalResponse.RentalId}");
        cancelResponse.EnsureSuccessStatusCode();

        Assert.Equal(System.Net.HttpStatusCode.NoContent, cancelResponse.StatusCode);

        // --- Consulta disponibilidad después de cancelar ---
        var availabilityResponseAfterCancel = await _client.GetAsync(availabilityUri);
        availabilityResponseAfterCancel.EnsureSuccessStatusCode();

        var carsAfterCancel = await availabilityResponseAfterCancel.Content.ReadFromJsonAsync<List<CarAvailabilityDto>>();

        Assert.NotNull(carsAfterCancel);
        Assert.NotEmpty(carsAfterCancel);

        // El auto debe estar disponible porque cancelamos la reserva anterior
        var availableCarAfterCancel = carsAfterCancel.FirstOrDefault(c => c.Id == carId);
        Assert.NotNull(availableCarAfterCancel);

        // --- Volver a reservar el mismo período ---
        var rebookResponse = await _client.PostAsJsonAsync("/api/rental", rentalDto);

        rebookResponse.EnsureSuccessStatusCode();

        //var secondRentalId = await rebookResponse.Content.ReadFromJsonAsync<Guid>();
        var secondRentalResponse = await rebookResponse.Content.ReadFromJsonAsync<CreateRentalResponseDto>();
        
        Assert.NotNull(secondRentalResponse);
        Assert.NotEqual(firstRentalResponse.RentalId, secondRentalResponse.RentalId);
    }
}
