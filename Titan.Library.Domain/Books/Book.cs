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