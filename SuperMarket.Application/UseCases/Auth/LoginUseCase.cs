using SuperMarket.Application.DTOs;
using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<UserLoginResponseDto> ExecuteAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var (accessToken, accessExpiresAt) = _tokenService.GenerateAccessToken(user);
        var (refreshTokenValue, refreshExpiresAt) = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken(user.Id, refreshTokenValue, refreshExpiresAt);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return new UserLoginResponseDto(accessToken, refreshTokenValue, accessExpiresAt);
    }
}
