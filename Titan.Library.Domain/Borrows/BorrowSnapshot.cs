namespace Titan.Library.Domain.Borrows;

public sealed class BorrowSnapshot
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public int BookId { get; init; }
    public DateTime BorrowedAt { get; init; }
    public DateTime? ReturnedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
