/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Tests.Functional.Common;

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;

namespace CarRental.Tests.Functional.Auth;

[Collection("Sequential")]
public class LoginTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task should_login_with_valid_credentials_and_receive_token()
    {
        // Arrange
        var email = "login@test.com";
        var password = "P@ssword123";

        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password,
            FullName = "Login Tester"
        });

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.False(string.IsNullOrEmpty(content?.Token));
    }

    [Fact]
    public async Task should_fail_login_with_wrong_password()
    {
        // Arrange
        var email = "wrongpass@test.com";
        var password = "Correct123";

        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password,
            FullName = "Wrong Pass User"
        });

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = "WrongPassword"
        });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task should_fail_login_with_nonexistent_email()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "nonexistent@test.com",
            Password = "DoesntMatter123"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task should_fail_login_with_empty_fields()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { Email = "", Password = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // si se valida con DTOs
    }

    [Fact]
    public async Task should_fail_login_with_invalid_email_format()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "not-an-email",
            Password = "Whatever123"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // si se usa [EmailAddress]
    }

    [Fact]
    public async Task should_include_claims_in_jwt()
    {
        var email = "claims@test.com";
        var password = "P@ssword123";

        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password,
            FullName = "Claim User"
        });

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        var content = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(content?.Token);

        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Email && c.Value == email);
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
        Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    private class LoginResponse
    {
        public string Token { get; set; } = "";
    }
}
