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
    public DateTime BorrowedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }

    public bool IsReturned => ReturnedAt.HasValue;

    public void Return()
    {
        if (IsReturned)
            throw new InvalidOperationException("Borrow already returned.");
        ReturnedAt = DateTime.UtcNow;
    }

    public BorrowSnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            CustomerId = CustomerId,
            BookId = BookId,
            BorrowedAt = BorrowedAt,
            ReturnedAt = ReturnedAt,
            CreatedAt = CreatedAt,
        };

    public static Borrow Reconstitute(BorrowSnapshot snapshot) =>
        new()
        {
            Id = snapshot.Id,
            CustomerId = snapshot.CustomerId,
            BookId = snapshot.BookId,
            BorrowedAt = snapshot.BorrowedAt,
            ReturnedAt = snapshot.ReturnedAt,
            CreatedAt = snapshot.CreatedAt,
        };

    public static Borrow Create(int customerId, int bookId) =>
        new()
        {
            CustomerId = customerId,
            BookId = bookId,
            BorrowedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
}
