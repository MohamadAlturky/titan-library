using Titan.Library.Domain.Books;

namespace Titan.Library.Domain.Users;

public class Author : User
{
    public Author() => RestoreUserType(UserType.Author);

    public List<Book> Books { get; set; } = [];

    public UserSnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            Name = Name,
            Email = Email,
            PasswordHash = PasswordHash,
            PasswordSalt = PasswordSalt,
            CreatedAt = CreatedAt,
            IsDeleted = IsDeleted,
            IsActive = IsActive,
            UserType = UserType,
        };

    public static Author Reconstitute(UserSnapshot snapshot)
    {
        var a = new Author
        {
            Id = snapshot.Id,
            Name = snapshot.Name,
            Email = snapshot.Email,
            CreatedAt = snapshot.CreatedAt,
        };
        a.RestorePassword(snapshot.PasswordHash, snapshot.PasswordSalt);
        a.RestoreIsDeleted(snapshot.IsDeleted);
        a.RestoreIsActive(snapshot.IsActive);
        a.RestoreUserType(snapshot.UserType);
        return a;
    }
}
