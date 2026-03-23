using Titan.Library.Domain.Books;

namespace Titan.Library.Domain.Users;

public class Author : User
{
    private Author()
        : base()
    {
        RestoreUserType(UserType.Author);
    }

    private readonly List<Book> _books = [];
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    public void AddBook(Book book) => _books.Add(book);

    public void ClearBooks() => _books.Clear();

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

    public static Author Create(string name, string email, string password)
    {
        var a = new Author
        {
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow,
        };
        a.SetPassword(password);
        return a;
    }

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
