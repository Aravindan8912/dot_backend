using System.Net;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperMarket.IntegrationTests.TestHelpers;

namespace SuperMarket.IntegrationTests.Integration;

[TestClass]
public class ProductsIntegrationTests
{
    private static SuperMarketWebApplicationFactory _factory = null!;
    private static HttpClient _client = null!;
    private static string _adminToken = null!;
    private static Guid _categoryId;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext _)
    {
        _factory = new SuperMarketWebApplicationFactory();
        _client = _factory.CreateClient();
        _adminToken = await HttpTestHelpers.GetAdminAccessTokenAsync(_client);

        // Ensure we have a category for product tests
        using var createCat = new HttpRequestMessage(HttpMethod.Post, "/api/categories");
        createCat.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        createCat.Content = JsonContent.Create(new { name = "Products Test Category " + Guid.NewGuid().ToString("N")[..8] });
        var catRes = await _factory.CreateClient().SendAsync(createCat);
        catRes.EnsureSuccessStatusCode();
        var catJson = await catRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        _categoryId = catJson.GetProperty("id").GetGuid();
    }

    [ClassCleanup]
    public static void ClassCleanup() => _factory?.Dispose();

    [TestMethod]
    public async Task GetAll_WithToken_Returns200()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/api/products");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        var response = await _client.SendAsync(req);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task Create_WithValidData_Returns200AndProduct()
    {
        var name = "Integration Product " + Guid.NewGuid().ToString("N")[..8];
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/products");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        req.Content = JsonContent.Create(new { name, price = 9.99m, stock = 50, categoryId = _categoryId });
        var response = await _client.SendAsync(req);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        Assert.AreEqual(name, json.GetProperty("name").GetString());
        Assert.AreEqual(9.99m, json.GetProperty("price").GetDecimal());
        Assert.AreEqual(50, json.GetProperty("stock").GetInt32());
    }

    [TestMethod]
    public async Task Create_DuplicateNameInSameCategory_Returns409()
    {
        var name = "Duplicate Name " + Guid.NewGuid().ToString("N")[..8];
        using var req1 = new HttpRequestMessage(HttpMethod.Post, "/api/products");
        req1.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        req1.Content = JsonContent.Create(new { name, price = 1m, stock = 10, categoryId = _categoryId });
        var res1 = await _client.SendAsync(req1);
        res1.EnsureSuccessStatusCode();

        using var req2 = new HttpRequestMessage(HttpMethod.Post, "/api/products");
        req2.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        req2.Content = JsonContent.Create(new { name, price = 2m, stock = 20, categoryId = _categoryId });
        var res2 = await _client.SendAsync(req2);

        Assert.AreEqual(HttpStatusCode.Conflict, res2.StatusCode);
    }

    [TestMethod]
    public async Task GetById_WithValidId_Returns200()
    {
        var name = "GetById Product " + Guid.NewGuid().ToString("N")[..8];
        using var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/products");
        createReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        createReq.Content = JsonContent.Create(new { name, price = 5m, stock = 25, categoryId = _categoryId });
        var createRes = await _client.SendAsync(createReq);
        createRes.EnsureSuccessStatusCode();
        var created = await createRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        var id = created.GetProperty("id").GetGuid();

        using var getReq = new HttpRequestMessage(HttpMethod.Get, $"/api/products/{id}");
        getReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        var getRes = await _client.SendAsync(getReq);

        Assert.AreEqual(HttpStatusCode.OK, getRes.StatusCode);
        var getJson = await getRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        Assert.AreEqual(id, getJson.GetProperty("id").GetGuid());
        Assert.AreEqual(name, getJson.GetProperty("name").GetString());
    }

    [TestMethod]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"/api/products/{Guid.NewGuid()}");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _adminToken);
        var response = await _client.SendAsync(req);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
