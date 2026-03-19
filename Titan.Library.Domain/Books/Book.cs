using Titan.Library.Common.Abstractions;
using Titan.Library.Domain.Borrows;

namespace Titan.Library.Domain.Books;

public class Book : BaseEntity<int>
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<Borrow> Borrows { get; set; } = [];
    public List<BookQuantityTransactionHistory> TransactionHistories { get; set; } = [];
}

public class BookQuantityTransactionHistory : BaseEntity<int>
{
    public int Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}

public enum TransactionType
{
    AddingToTheStore = 1,
    BookReturned = 2,
    BookBorrowed = 3,
}