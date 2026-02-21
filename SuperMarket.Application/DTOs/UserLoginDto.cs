namespace SuperMarket.Application.DTOs;

public record UserLoginDto(string Email, string Password);

public record UserLoginResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc
);

public record RefreshTokenRequest(string RefreshToken);