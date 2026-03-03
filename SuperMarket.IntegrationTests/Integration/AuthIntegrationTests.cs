using System.Net;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperMarket.IntegrationTests.TestHelpers;

namespace SuperMarket.IntegrationTests.Integration;

[TestClass]
public class AuthIntegrationTests
{
    private static SuperMarketWebApplicationFactory _factory = null!;
    private static HttpClient _client = null!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        _factory = new SuperMarketWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [ClassCleanup]
    public static void ClassCleanup() => _factory?.Dispose();

    [TestMethod]
    public async Task Login_WithValidAdminCredentials_Returns200AndTokens()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@supermarket.local",
            password = "Admin@123"
        });

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        Assert.IsTrue(json.TryGetProperty("accessToken", out _));
        Assert.IsTrue(json.TryGetProperty("refreshToken", out _));
    }

    [TestMethod]
    public async Task Login_WithInvalidPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@supermarket.local",
            password = "WrongPassword"
        });

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Login_WithEmptyEmail_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "",
            password = "Admin@123"
        });

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task Login_WithNullBody_Returns400()
    {
        var response = await _client.PostAsync("/api/auth/login", null);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
