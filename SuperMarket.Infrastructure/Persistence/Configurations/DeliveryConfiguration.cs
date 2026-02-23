using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.OrderId).IsRequired();
        builder.Property(d => d.CustomerId).IsRequired();
        builder.Property(d => d.AddressId).IsRequired();
        builder.Property(d => d.PaymentId).IsRequired();
        builder.Property(d => d.Status).IsRequired().HasConversion<int>();
    }
}
