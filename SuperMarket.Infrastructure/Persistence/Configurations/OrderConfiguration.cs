using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;


namespace SuperMarket.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>{
    public void Configure(EntityTypeBuilder<Order> builder){
        builder.HasKey(o => o.Id);
        builder.Property(o => o.CustomerId).IsRequired();
        builder.Property(o => o.Status).IsRequired();
        builder.Property(o => o.OrderDate).IsRequired();
        builder.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId);
    }
}