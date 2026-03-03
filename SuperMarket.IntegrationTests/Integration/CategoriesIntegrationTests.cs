using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperMarket.IntegrationTests.TestHelpers;

namespace SuperMarket.IntegrationTests.Integration;

[TestClass]
public class CategoriesIntegrationTests
{
    private static SuperMarketWebApplicationFactory _factory = null!;
    private static HttpClient _client = null!;
    private static string _adminToken = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext _)
    {
        _factory = new SuperMarketWebApplicationFactory();
        _client = _factory.CreateClient();
        _adminToken = await HttpTestHelpers.GetAdminAccessTokenAsync(_client);
    }

    [ClassCleanup]
    public static void ClassCleanup() => _factory?.Dispose();

    [TestMethod]
    public async Task GetAll_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/categories");
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GetAll_WithToken_Returns200AndArray()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/api/categories");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        var response = await _client.SendAsync(req);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        Assert.IsTrue(json.ValueKind == JsonValueKind.Array);
    }

    [TestMethod]
    public async Task Create_WithValidName_Returns200AndCategory()
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/categories");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        req.Content = JsonContent.Create(new { name = "Integration Test Category " + Guid.NewGuid().ToString("N")[..8] });
        var response = await _client.SendAsync(req);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        Assert.IsTrue(json.TryGetProperty("id", out _));
        Assert.IsTrue(json.TryGetProperty("name", out _));
    }

    [TestMethod]
    public async Task GetById_WithValidId_Returns200()
    {
        // Create a category first
        using var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/categories");
        createReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        createReq.Content = JsonContent.Create(new { name = "GetById Test " + Guid.NewGuid().ToString("N")[..8] });
        var createRes = await _client.SendAsync(createReq);
        createRes.EnsureSuccessStatusCode();
        var created = await createRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        var id = created.GetProperty("id").GetGuid();

        using var getReq = new HttpRequestMessage(HttpMethod.Get, $"/api/categories/{id}");
        getReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        var getRes = await _client.SendAsync(getReq);

        Assert.AreEqual(HttpStatusCode.OK, getRes.StatusCode);
        var getJson = await getRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        Assert.AreEqual(id, getJson.GetProperty("id").GetGuid());
    }

    [TestMethod]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        var fakeId = Guid.NewGuid();
        using var req = new HttpRequestMessage(HttpMethod.Get, $"/api/categories/{fakeId}");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        var response = await _client.SendAsync(req);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
