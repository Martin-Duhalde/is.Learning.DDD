/// MIT License © 2025 Martín Duhalde + ChatGPT

using System.Net.Http.Json;

namespace CarRental.FunctionalTests.Auth;

public static class AuthHelper
{
    public static async Task<string> RegisterAndLoginAsync(HttpClient client, string email, string password)
    {
        // Registro
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password,
            FullName = "Test User"
        });
        registerResponse.EnsureSuccessStatusCode();

        // Login
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });
        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        return loginContent?.Token ?? string.Empty;
    }

    private class LoginResponse
    {
        public string Token { get; set; } = "";
    }
}
