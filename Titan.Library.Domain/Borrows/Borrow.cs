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
}
