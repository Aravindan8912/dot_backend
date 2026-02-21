using SuperMarket.Application.DTOs;
using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Auth;

public class RefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<UserLoginResponseDto> ExecuteAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var stored = await _refreshTokenRepository.GetByTokenAsync(refreshTokenValue, cancellationToken);
        if (stored == null || !stored.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var user = await _userRepository.GetByIdAsync(stored.UserId, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("User not found.");

        stored.Revoke();
        await _refreshTokenRepository.UpdateAsync(stored, cancellationToken);

        var (accessToken, accessExpiresAt) = _tokenService.GenerateAccessToken(user);
        var (newRefreshValue, newRefreshExpires) = _tokenService.GenerateRefreshToken();
        var newRefresh = new RefreshToken(user.Id, newRefreshValue, newRefreshExpires);
        await _refreshTokenRepository.AddAsync(newRefresh, cancellationToken);

        return new UserLoginResponseDto(accessToken, newRefreshValue, accessExpiresAt);
    }
}
