using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Requests;
using SuperMarket.Application.UseCases.Auth;
using SuperMarket.Domain.Enums;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly RegisterUserUseCase _registerUserUseCase;

    public AuthController(
        LoginUseCase loginUseCase,
        RefreshTokenUseCase refreshTokenUseCase,
        RegisterUserUseCase registerUserUseCase)
    {
        _loginUseCase = loginUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _registerUserUseCase = registerUserUseCase;
    }

    /// <summary>Login with email and password. Returns access and refresh tokens.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and password are required.");

        try
        {
            var result = await _loginUseCase.ExecuteAsync(request.Email, request.Password, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    /// <summary>Exchange a valid refresh token for a new access token and refresh token (rotation).</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("RefreshToken is required.");

        try
        {
            var result = await _refreshTokenUseCase.ExecuteAsync(request.RefreshToken, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    /// <summary>Register a new user. Admin only. Role: "Admin" or "User".</summary>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and password are required.");

        var role = request.Role?.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase) == true ? Role.Admin : Role.User;

        try
        {
            var user = await _registerUserUseCase.ExecuteAsync(request.Email, request.Password, role, cancellationToken);
            return Ok(new { user.Id, user.Email, Role = role.ToString() });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
