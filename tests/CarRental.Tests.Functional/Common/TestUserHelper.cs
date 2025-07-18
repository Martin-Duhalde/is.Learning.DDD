namespace CarRental.Tests.Functional.Common;

using System.Net.Http.Json;

public static class TestUserHelper
{
    public static async Task<string> GetValidTokenAsync(HttpClient client, string email = "testuser@test.com", string password = "Test1234!")
    {
        // Intentar registrar usuario
        await client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password,
            FullName = "Test User"
        });

        // Login
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (content is null || string.IsNullOrEmpty(content.Token))
            throw new Exception("Token inválido al intentar loguear");

        return content.Token;
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}

