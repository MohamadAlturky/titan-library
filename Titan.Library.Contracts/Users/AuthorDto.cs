using Titan.Library.Common.Dtos;
using Titan.Library.Domain.Users;

namespace Titan.Library.Contracts.Users;

public class AuthorDto : BaseDto<Author, int>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public override void Map(Author entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Email = entity.Email;
        CreatedAt = entity.CreatedAt;
    }
}
