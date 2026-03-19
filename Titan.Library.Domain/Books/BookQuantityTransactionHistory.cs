using Titan.Library.Common.Abstractions;

namespace Titan.Library.Domain.Books;

public class BookQuantityTransactionHistory : BaseEntity<int>
{
    public int Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public BookTransactionHistorySnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            BookId = BookId,
            Amount = Amount,
            TransactionType = (int)TransactionType,
            CreatedAt = CreatedAt,
        };

    public static BookQuantityTransactionHistory Reconstitute(
        BookTransactionHistorySnapshot snapshot
    ) =>
        new()
        {
            Id = snapshot.Id,
            BookId = snapshot.BookId,
            Amount = snapshot.Amount,
            TransactionType = (TransactionType)snapshot.TransactionType,
            CreatedAt = snapshot.CreatedAt,
        };
}

public enum TransactionType
{
    AddingToTheStore = 1,
    BookReturned = 2,
    BookBorrowed = 3,
}
