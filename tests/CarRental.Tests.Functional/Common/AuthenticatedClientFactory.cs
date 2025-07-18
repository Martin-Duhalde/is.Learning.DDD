namespace CarRental.Tests.Functional.Common;
public static class AuthenticatedClientFactory
{
    public static async Task<HttpClient> CreateAuthenticatedClientAsync(CustomWebApplicationFactory<Program> factory, string email = "testuser@test.com")
    {
        var client = factory.CreateClient();

        var token = await TestUserHelper.GetValidTokenAsync(client, email);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
