using SuperMarket.Domain.Common;

namespace SuperMarket.Domain.Entities;

/// <summary>
/// Stored refresh token for rotation and revocation (production-grade).
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByToken { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string? replacedByToken = null)
    {
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}
