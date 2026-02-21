using Microsoft.Extensions.DependencyInjection;
using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Infrastructure;

public static class SeedData
{
    /// <summary>
    /// Seeds default admin user when no user exists. For development only.
    /// </summary>
    public static async Task SeedDefaultAdminIfNeededAsync(this IServiceProvider serviceProvider)
    {
        var userRepo = serviceProvider.GetRequiredService<IUserRepository>();
        var hasher = serviceProvider.GetRequiredService<IPasswordHasher>();
        var existing = await userRepo.GetByEmailAsync("admin@supermarket.local");
        if (existing != null)
            return;
        var admin = new User(
            "admin@supermarket.local",
            hasher.Hash("Admin@123"),
            Role.Admin);
        await userRepo.AddAsync(admin);
    }
}
