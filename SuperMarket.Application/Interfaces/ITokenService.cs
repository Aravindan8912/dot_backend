using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.Interfaces;

/// <summary>
/// Production-grade token service: access JWT + refresh token with expiry.
/// </summary>
public interface ITokenService
{
    (string AccessToken, DateTime ExpiresAtUtc) GenerateAccessToken(User user);
    (string Token, DateTime ExpiresAtUtc) GenerateRefreshToken();
    Guid? ValidateAccessToken(string token);
}
