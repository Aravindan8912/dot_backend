using SuperMarket.Application.Interfaces;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.UseCases.Customers;

public class CreateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateCustomerUseCase(
        ICustomerRepository customerRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _customerRepository = customerRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Customer> ExecuteAsync(
        string name,
        string email,
        string phone,
        string? password = null,
        string? role = null,
        CancellationToken cancellationToken = default)
    {
        Guid? userId = null;

        if (!string.IsNullOrWhiteSpace(password))
        {
            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser != null)
                throw new InvalidOperationException("A user with this email already exists.");

            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters.");

            var roleEnum = ParseRole(role);
            var user = new User(email, _passwordHasher.Hash(password), roleEnum);
            await _userRepository.AddAsync(user, cancellationToken);
            userId = user.Id;
        }

        var customer = new Customer(name, email, phone, userId);
        await _customerRepository.AddAsync(customer);
        return customer;
    }

    private static Role ParseRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return Role.User;
        return role.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase) ? Role.Admin : Role.User;
    }
}
