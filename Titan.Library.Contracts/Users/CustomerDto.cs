using Titan.Library.Domain.Users;

namespace Titan.Library.Contracts.Users;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public static CustomerDto FromEntity(Customer entity) => new()
    {
        Id = entity.Id, Name = entity.Name, Email = entity.Email, CreatedAt = entity.CreatedAt,
    };
}
