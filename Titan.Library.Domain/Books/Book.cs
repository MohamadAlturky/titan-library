using Titan.Library.Common.Abstractions;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Users;

namespace Titan.Library.Domain.Books;

public class Book : BaseEntity<int>
{
    private Book() { }

    public string Isbn { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public bool IsDeleted { get; private set; }

    public void Delete() => IsDeleted = true;

    public Author Author { get; set; } = null!;

    private readonly List<Borrow> _borrows = [];
    public IReadOnlyCollection<Borrow> Borrows => _borrows.AsReadOnly();

    public void AddBorrow(Borrow borrow) => _borrows.Add(borrow);

    public void ClearBorrows() => _borrows.Clear();

    public BookSnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            Isbn = Isbn,
            AuthorId = AuthorId,
            Title = Title,
            Description = Description,
            CreatedAt = CreatedAt,
            IsAvailable = IsAvailable,
            IsDeleted = IsDeleted,
        };

    public static Book Create(int authorId, string isbn, string title, string description) =>
        new()
        {
            AuthorId = authorId,
            Isbn = isbn,
            Title = title,
            Description = description,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

    public static Book Reconstitute(BookSnapshot snapshot) =>
        new()
        {
            Id = snapshot.Id,
            Isbn = snapshot.Isbn,
            AuthorId = snapshot.AuthorId,
            Title = snapshot.Title,
            Description = snapshot.Description,
            CreatedAt = snapshot.CreatedAt,
            IsAvailable = snapshot.IsAvailable,
            IsDeleted = snapshot.IsDeleted,
        };
}
