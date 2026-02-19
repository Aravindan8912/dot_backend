using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;


namespace SuperMarket.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>{
    public void Configure(EntityTypeBuilder<OrderItem> builder){
        builder.HasKey(i => i.Id);
        builder.Property(i => i.OrderId).IsRequired();
        builder.Property(i => i.ProductId).IsRequired();
        
        builder.Property(i => i.Price)
        .IsRequired()
        .HasPrecision(18, 2);


        builder.Property(i => i.Quantity)
        .IsRequired();
    }
}