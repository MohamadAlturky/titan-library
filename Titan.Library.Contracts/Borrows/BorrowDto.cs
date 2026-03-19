using Titan.Library.Common.Dtos;
using Titan.Library.Domain.Borrows;

namespace Titan.Library.Contracts.Borrows;

public class BorrowDto : BaseDto<Borrow, int>
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int BookId { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public bool IsReturned { get; set; }
    public DateTime CreatedAt { get; set; }

    public override void Map(Borrow entity)
    {
        Id = entity.Id;
        CustomerId = entity.CustomerId;
        BookId = entity.BookId;
        BorrowedAt = entity.BorrowedAt;
        ReturnedAt = entity.ReturnedAt;
        IsReturned = entity.IsReturned;
        CreatedAt = entity.CreatedAt;
    }
}
