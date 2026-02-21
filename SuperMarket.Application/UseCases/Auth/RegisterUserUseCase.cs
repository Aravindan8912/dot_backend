using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.UseCases.Auth;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> ExecuteAsync(string email, string password, Role role, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters.");

        var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var hash = _passwordHasher.Hash(password);
        var user = new User(email, hash, role);
        await _userRepository.AddAsync(user, cancellationToken);
        return user;
    }
}
