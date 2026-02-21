using SuperMarket.Domain.Common;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Role Role { get; private set; } = Role.User;

    private User(){}

    public User(string email, string passwordHash, Role role)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }
}