using Titan.Library.Domain.Borrows;

namespace Titan.Library.Domain.Users;

public class Customer : User
{
    public List<Borrow> Borrows { get; set; } = [];

    public static Customer Reconstitute(
        int id,
        string name,
        string email,
        string passwordHash,
        string passwordSalt,
        DateTime createdAt
    )
    {
        var c = new Customer
        {
            Id = id,
            Name = name,
            Email = email,
            CreatedAt = createdAt,
        };
        c.RestorePassword(passwordHash, passwordSalt);
        return c;
    }

    public bool HasActiveBorrowForBook(int bookId) =>
        Borrows.Any(b => b.BookId == bookId && !b.IsReturned);
}
