/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Tests.Functional.Common;
using CarRental.UseCases.Auth.Dtos;

using System.Net.Http.Json;

namespace CarRental.Tests.Functional.Auth;

[Collection("Sequential")]
public class AuthTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();


    [Fact]
    public async Task should_be_accessible_without_token_on_public_endpoint()
    {
        // Act
        var response = await _client.GetAsync("/api/authtest/public");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("✅ Acceso público", content);
    }
    [Fact]
    public async Task should_register_successfully_when_data_is_valid()
    {
        // Arrange
        var dto = new RegisterDto
        {
            FullName = "FuncTest User",
            Email = $"test{System.Guid.NewGuid():N}@mail.com",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        // Assert
        response.EnsureSuccessStatusCode(); // HTTP 200 OK or 201 Created
    }
}

