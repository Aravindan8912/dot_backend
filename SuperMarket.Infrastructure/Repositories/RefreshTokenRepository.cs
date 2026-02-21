using Microsoft.EntityFrameworkCore;
using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;
using SuperMarket.Infrastructure.Persistence;

namespace SuperMarket.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context) => _context = context;

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken);

    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);
        foreach (var t in tokens)
            t.Revoke();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
