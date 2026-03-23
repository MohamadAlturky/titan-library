using Titan.Library.Common.Abstractions;
using Titan.Library.Domain.Books;
using Titan.Library.Domain.Users;

namespace Titan.Library.Domain.Borrows;

public class Borrow : BaseEntity<int>
{
    public int CustomerId { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public DateTime? ReturnedAt { get; set; }

    public bool IsReturned { get; set; }

    public void Return()
    {
        if (IsReturned)
            throw new InvalidOperationException("Borrow already returned.");
        ReturnedAt = DateTime.UtcNow;
        IsReturned = true;
    }

    public BorrowSnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            CustomerId = CustomerId,
            BookId = BookId,
            ReturnedAt = ReturnedAt,
            CreatedAt = CreatedAt,
            IsReturned = IsReturned,
        };

    public static Borrow Reconstitute(BorrowSnapshot snapshot) =>
        new()
        {
            Id = snapshot.Id,
            CustomerId = snapshot.CustomerId,
            BookId = snapshot.BookId,
            IsReturned = snapshot.IsReturned,
            ReturnedAt = snapshot.ReturnedAt,
            CreatedAt = snapshot.CreatedAt,
        };

    public static Borrow Create(int customerId, int bookId) =>
        new()
        {
            CustomerId = customerId,
            BookId = bookId,
            CreatedAt = DateTime.UtcNow,
            IsReturned = false,
        };
}
