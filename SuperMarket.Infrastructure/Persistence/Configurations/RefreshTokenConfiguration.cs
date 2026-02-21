using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Token).IsRequired().HasMaxLength(512);
        builder.HasIndex(r => r.Token);
        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.ExpiresAtUtc).IsRequired();
        builder.Property(r => r.CreatedAtUtc).IsRequired();
        builder.Property(r => r.RevokedAtUtc);
        builder.Property(r => r.ReplacedByToken).HasMaxLength(512);
    }
}
