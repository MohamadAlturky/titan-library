using Titan.Library.Domain.Borrows;

namespace Titan.Library.Contracts.Borrows;

public class BorrowDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int BookId { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public bool IsReturned { get; set; }
    public DateTime CreatedAt { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;

    public static BorrowDto FromEntity(Borrow entity) =>
        new()
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            BookId = entity.BookId,
            ReturnedAt = entity.ReturnedAt,
            IsReturned = entity.IsReturned,
            CreatedAt = entity.CreatedAt,
        };
}
