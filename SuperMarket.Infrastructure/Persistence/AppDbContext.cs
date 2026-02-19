using Microsoft.EntityFrameworkCore;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence;

public class AppDbContext : DbContext{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
    : base(options){}


    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}