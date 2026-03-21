namespace Titan.Library.Domain.Users;

public sealed class UserSnapshot
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string PasswordSalt { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
    public bool IsActive { get; init; }
    public UserType UserType { get; init; }
}
