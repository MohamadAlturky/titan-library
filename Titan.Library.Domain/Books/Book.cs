using Titan.Library.Common.Abstractions;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Users;

namespace Titan.Library.Domain.Books;

public class Book : BaseEntity<int>
{
    public string Isbn { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;

    public Author Author { get; set; } = null!;
    public List<Borrow> Borrows { get; set; } = [];
    public List<BookQuantityTransactionHistory> TransactionHistories { get; set; } = [];

    public BookSnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            Isbn = Isbn,
            AuthorId = AuthorId,
            Title = Title,
            CreatedAt = CreatedAt,
        };

    public static Book Reconstitute(BookSnapshot snapshot) =>
        new()
        {
            Id = snapshot.Id,
            Isbn = snapshot.Isbn,
            AuthorId = snapshot.AuthorId,
            Title = snapshot.Title,
            CreatedAt = snapshot.CreatedAt,
        };

    public void AddToStock(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(amount));
        TransactionHistories.Add(
            new BookQuantityTransactionHistory
            {
                Amount = amount,
                TransactionType = TransactionType.AddingToTheStore,
                BookId = Id,
                CreatedAt = DateTime.UtcNow,
            }
        );
    }

    public bool IsAvailable() =>
        TransactionHistories.Sum(t =>
            t.TransactionType switch
            {
                TransactionType.AddingToTheStore => t.Amount,
                TransactionType.BookReturned => t.Amount,
                TransactionType.BookBorrowed => -t.Amount,
                _ => 0,
            }
        ) > 0;
}

