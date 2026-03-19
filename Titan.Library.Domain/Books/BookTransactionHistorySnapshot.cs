namespace Titan.Library.Domain.Books;

public sealed class BookTransactionHistorySnapshot
{
    public int Id { get; init; }
    public int BookId { get; init; }
    public int Amount { get; init; }
    public int TransactionType { get; init; }
    public DateTime CreatedAt { get; init; }
}
