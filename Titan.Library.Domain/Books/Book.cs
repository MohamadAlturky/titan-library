using Titan.Library.Common.Abstractions;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Users;

namespace Titan.Library.Domain.Books;

public class Book : BaseEntity<int>
{
    public string Isbn { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public bool IsDeleted { get; private set; }

    public void Delete() => IsDeleted = true;

    public Author Author { get; set; } = null!;
    public List<Borrow> Borrows { get; set; } = [];

    public BookSnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            Isbn = Isbn,
            AuthorId = AuthorId,
            Title = Title,
            CreatedAt = CreatedAt,
            IsAvailable = IsAvailable,
            IsDeleted = IsDeleted,
        };

    public static Book Reconstitute(BookSnapshot snapshot) =>
        new()
        {
            Id = snapshot.Id,
            Isbn = snapshot.Isbn,
            AuthorId = snapshot.AuthorId,
            Title = snapshot.Title,
            CreatedAt = snapshot.CreatedAt,
            IsAvailable = snapshot.IsAvailable,
            IsDeleted = snapshot.IsDeleted,
        };
}

