using Titan.Library.Domain.Books;

namespace Titan.Library.Domain.Users;

public class Author : User
{
    public List<Book> Books { get; set; } = [];

    public static Author Reconstitute(
        int id,
        string name,
        string email,
        string passwordHash,
        string passwordSalt,
        DateTime createdAt
    )
    {
        var a = new Author
        {
            Id = id,
            Name = name,
            Email = email,
            CreatedAt = createdAt,
        };
        a.RestorePassword(passwordHash, passwordSalt);
        return a;
    }
}
