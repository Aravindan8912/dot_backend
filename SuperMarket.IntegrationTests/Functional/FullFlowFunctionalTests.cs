using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperMarket.IntegrationTests.TestHelpers;

namespace SuperMarket.IntegrationTests.Functional;

/// <summary>
/// Functional tests: multi-step flows from a user/API consumer perspective.
/// </summary>
[TestClass]
public class FullFlowFunctionalTests
{
    private static SuperMarketWebApplicationFactory _factory = null!;
    private static HttpClient _client = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext _)
    {
        _factory = new SuperMarketWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [ClassCleanup]
    public static void ClassCleanup() => _factory?.Dispose();

    [TestMethod]
    public async Task FullFlow_LoginCreateCategoryCreateProductGetProducts_Succeeds()
    {
        // 1. Login as admin
        var token = await HttpTestHelpers.GetAdminAccessTokenAsync(_client);
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create a category
        var categoryName = "Functional Test Category " + Guid.NewGuid().ToString("N")[..8];
        var createCatRes = await _client.PostAsJsonAsync("/api/categories", new { name = categoryName });
        Assert.AreEqual(HttpStatusCode.OK, createCatRes.StatusCode);
        var category = await createCatRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        var categoryId = category.GetProperty("id").GetGuid();
        Assert.AreEqual(categoryName, category.GetProperty("name").GetString());

        // 3. Create a product in that category
        var productName = "Functional Test Product " + Guid.NewGuid().ToString("N")[..8];
        var createProdRes = await _client.PostAsJsonAsync("/api/products", new
        {
            name = productName,
            price = 12.99m,
            stock = 100,
            categoryId
        });
        Assert.AreEqual(HttpStatusCode.OK, createProdRes.StatusCode);
        var product = await createProdRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        var productId = product.GetProperty("id").GetGuid();
        Assert.AreEqual(productName, product.GetProperty("name").GetString());
        Assert.AreEqual(12.99m, product.GetProperty("price").GetDecimal());
        Assert.AreEqual(100, product.GetProperty("stock").GetInt32());

        // 4. Get all products and verify our product appears
        var getProductsRes = await _client.GetAsync("/api/products");
        Assert.AreEqual(HttpStatusCode.OK, getProductsRes.StatusCode);
        var productsArray = await getProductsRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        Assert.IsTrue(productsArray.ValueKind == JsonValueKind.Array);
        var found = false;
        foreach (var item in productsArray.EnumerateArray())
        {
            if (item.GetProperty("id").GetGuid() == productId)
            {
                found = true;
                Assert.AreEqual(productName, item.GetProperty("name").GetString());
                break;
            }
        }
        Assert.IsTrue(found, "Created product should appear in GET /api/products");
    }

    [TestMethod]
    public async Task FullFlow_LoginGetCategoriesCreateProductUpdateProduct_Succeeds()
    {
        var token = await HttpTestHelpers.GetAdminAccessTokenAsync(_client);
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Get or create a category
        var getCatRes = await _client.GetAsync("/api/categories");
        getCatRes.EnsureSuccessStatusCode();
        var categories = await getCatRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        Guid categoryId;
        var first = categories.EnumerateArray().FirstOrDefault();
        if (first.ValueKind != JsonValueKind.Undefined && first.ValueKind != JsonValueKind.Null)
            categoryId = first.GetProperty("id").GetGuid();
        else
        {
            var createCatRes = await _client.PostAsJsonAsync("/api/categories", new { name = "Flow Category " + Guid.NewGuid().ToString("N")[..8] });
            createCatRes.EnsureSuccessStatusCode();
            var newCat = await createCatRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
            categoryId = newCat.GetProperty("id").GetGuid();
        }

        // Create product
        var productName = "Update Flow Product " + Guid.NewGuid().ToString("N")[..8];
        var createRes = await _client.PostAsJsonAsync("/api/products", new
        {
            name = productName,
            price = 5.00m,
            stock = 10,
            categoryId
        });
        createRes.EnsureSuccessStatusCode();
        var product = await createRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        var productId = product.GetProperty("id").GetGuid();

        // Update product
        var updatedName = "Updated " + productName;
        var updateRes = await _client.PutAsJsonAsync($"/api/products/{productId}", new
        {
            name = updatedName,
            price = 7.50m,
            categoryId
        });
        Assert.AreEqual(HttpStatusCode.OK, updateRes.StatusCode);
        var updated = await updateRes.Content.ReadFromJsonAsync<JsonElement>(HttpTestHelpers.JsonOptions);
        Assert.AreEqual(updatedName, updated.GetProperty("name").GetString());
        Assert.AreEqual(7.50m, updated.GetProperty("price").GetDecimal());
        Assert.AreEqual(10, updated.GetProperty("stock").GetInt32()); // stock unchanged
    }

    [TestMethod]
    public async Task FullFlow_UnauthenticatedAccessToProtectedEndpoints_Returns401()
    {
        using var client = _factory.CreateClient();
        var resCategories = await client.GetAsync("/api/categories");
        Assert.AreEqual(HttpStatusCode.Unauthorized, resCategories.StatusCode);

        var resProducts = await client.GetAsync("/api/products");
        Assert.AreEqual(HttpStatusCode.Unauthorized, resProducts.StatusCode);
    }
}
