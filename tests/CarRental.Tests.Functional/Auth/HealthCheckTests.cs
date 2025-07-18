/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Tests.Functional.Common;

using System.Net;

namespace CarRental.Tests.Functional.Auth;

[Collection("Sequential")]
public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthCheckTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task should_return_200_on_alive()
    {
        var response = await _client.GetAsync("/alive");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

