using CarRental.UseCases.Cars.Create;

using System.Net;
using System.Net.Http.Json;

namespace CarRental.Tests.Functional.Cars;

/// <summary>
/// 🧪 Unit-style API controller tests for `/api/car` endpoint with clean structure.
/// </summary>
public class CarControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CarControllerTests(CustomWebApplicationFactory<Program> factory)
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

    [Fact(DisplayName = "✅ should get all cars when logged in")]
    public async Task should_get_all_cars_when_logged_in()
    {
        var response = await _client.GetAsync("/api/car");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "➕ should create a car (Toyota) successfully")]
    public async Task should_create_a_car_successfully()
    {
        var createRequest = new CreateCarRequestDto { Model = "Toyota Corolla", Type = "Sedan" };

        var response = await _client.PostAsJsonAsync("/api/car", createRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact(DisplayName = "➕ should create car and return valid Id")]
    public async Task should_create_car_successfully()
    {
        var createRequest = new CreateCarRequestDto { Model = "Tesla Model 3", Type = "Sedan" };

        var response = await _client.PostAsJsonAsync("/api/car", createRequest);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CreateCarResponseDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result!.CarId);
    }

    [Fact(DisplayName = "✅ should get all cars")]
    public async Task should_get_all_cars()
    {
        var response = await _client.GetAsync("/api/car");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "🚫 should fail on duplicate car")]
    public async Task should_fail_on_duplicate_car()
    {
        var dto = new CreateCarRequestDto { Model = "Nissan Leaf", Type = "Hatchback" };

        await _client.PostAsJsonAsync("/api/car", dto);
        var duplicate = await _client.PostAsJsonAsync("/api/car", dto);

        Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode);
    }

    [Fact(DisplayName = "🚫 should fail on invalid car data")]
    public async Task should_fail_on_invalid_data()
    {
        var invalidDto = new CreateCarRequestDto { Model = "", Type = "" };

        var response = await _client.PostAsJsonAsync("/api/car", invalidDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "🚫 should return 400 when creating car with invalid data")]
    public async Task should_return_400_when_creating_car_with_invalid_data()
    {
        var invalidDto = new CreateCarRequestDto { Model = "", Type = "" };

        var response = await _client.PostAsJsonAsync("/api/car", invalidDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
