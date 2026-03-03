using System.Net.Http.Json;
using System.Text.Json;

namespace SuperMarket.IntegrationTests.TestHelpers;

public static class HttpTestHelpers
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Log in as the seeded admin user and return the access token.
    /// </summary>
    public static async Task<string> GetAdminAccessTokenAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@supermarket.local",
            password = "Admin@123"
        });
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        return json.GetProperty("accessToken").GetString() ?? throw new InvalidOperationException("No accessToken in login response.");
    }

    public static HttpClient WithBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
