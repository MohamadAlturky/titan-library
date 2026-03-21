using Titan.Library.Domain.Users;

namespace Titan.Library.Contracts.Users;

public class AuthorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public static AuthorDto FromEntity(Author entity) => new()
    {
        Id = entity.Id, Name = entity.Name, Email = entity.Email, CreatedAt = entity.CreatedAt, IsActive = entity.IsActive,
    };
}
