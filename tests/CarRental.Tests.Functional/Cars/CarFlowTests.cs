using CarRental.UseCases.Cars.Create;
using CarRental.UseCases.Cars.Dtos;

using System.Net;
using System.Net.Http.Json;

namespace CarRental.Tests.Functional.Cars;

/// <summary>
/// 🌐 Test funcionales del flujo completo de `/api/car`, validando endpoints reales y comportamiento del sistema.
/// </summary>
public class CarFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    /// <summary>
    /// 🔧 Inicializa el cliente HTTP usando la factory con host configurado para testeo real.
    /// </summary>
    public CarFlowTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "❤️ should return 200 on /alive")]
    public async Task should_return_200_on_alive()
    {
        var response = await _client.GetAsync("/alive");
        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "✅ should get all cars")]
    public async Task should_get_all_cars()
    {
        // --- Arrange: Crear dos autos para asegurar que la lista no esté vacía ---
        var car1 = new CreateCarRequestDto { Model = "Test Model 1", Type = "Sedan" };
        var car2 = new CreateCarRequestDto { Model = "Test Model 2", Type = "SUV" };

        var response1 = await _client.PostAsJsonAsync("/api/car", car1);
        var response2 = await _client.PostAsJsonAsync("/api/car", car2);

        response1.EnsureSuccessStatusCode();
        response2.EnsureSuccessStatusCode();

        // --- Act: Obtener la lista completa de autos ---
        var getAllResponse = await _client.GetAsync("/api/car");

        // --- Assert: verificar estado 200 OK ---
        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);

        // --- Leer la lista de autos devuelta ---
        var cars = await getAllResponse.Content.ReadFromJsonAsync<List<CarDto>>();

        Assert.NotNull(cars);
        Assert.NotEmpty(cars);

        // --- Verificar que los autos creados estén en la lista ---
        Assert.Contains(cars, c => c.Model == car1.Model && c.Type == car1.Type);
        Assert.Contains(cars, c => c.Model == car2.Model && c.Type == car2.Type);
    }
    [Fact(DisplayName = "➕ should create car successfully")]
    public async Task should_create_car_successfully()
    {
        var request = new CreateCarRequestDto { Model = "Ford Focus", Type = "Hatchback" };

        // Cambié la ruta a /api/car para que coincida con el controlador
        var response = await _client.PostAsJsonAsync("/api/car", request);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CreateCarResponseDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result!.CarId);
    }

    [Fact(DisplayName = "🚫 should fail on duplicate car")]
    public async Task should_fail_on_duplicate_car()
    {
        var dto = new CreateCarRequestDto{ Model = "Nissan Leaf", Type = "Electric" };

        await _client.PostAsJsonAsync("/api/car", dto);
        var duplicate = await _client.PostAsJsonAsync("/api/car", dto);

        Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode);
    }

    [Fact(DisplayName = "🚫 should fail on invalid car data")]
    public async Task should_fail_on_invalid_data()
    {
        var invalidDto = new CreateCarRequestDto{ Model = "", Type = "" };

        var response = await _client.PostAsJsonAsync("/api/car", invalidDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    
}
