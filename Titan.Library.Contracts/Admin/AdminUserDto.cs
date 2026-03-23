using Titan.Library.Domain.Users;

namespace Titan.Library.Contracts.Admin;

public class AdminUserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public static AdminUserDto FromEntity(User entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            UserType = (int)entity.UserType,
            CreatedAt = entity.CreatedAt,
            IsActive = entity.IsActive,
        };
}
