using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SuperMarket.Infrastructure.Persistence;

namespace SuperMarket.IntegrationTests;

/// <summary>
/// Factory for integration and functional tests. Uses an in-memory database and test JWT config.
/// </summary>
public class SuperMarketWebApplicationFactory : WebApplicationFactory<SuperMarket.API.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "IntegrationTestSecretKeyAtLeast32CharactersLong!",
                ["Jwt:Issuer"] = "SuperMarket.Test",
                ["Jwt:Audience"] = "SuperMarket.Test"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(AppDbContext));
            if (dbDescriptor != null)
                services.Remove(dbDescriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestDb"));
        });
    }
}
