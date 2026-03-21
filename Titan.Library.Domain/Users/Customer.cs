using Titan.Library.Domain.Borrows;

namespace Titan.Library.Domain.Users;

public class Customer : User
{
    public Customer() => RestoreUserType(UserType.Customer);

    public List<Borrow> Borrows { get; set; } = [];

    public UserSnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            Name = Name,
            Email = Email,
            PasswordHash = PasswordHash,
            PasswordSalt = PasswordSalt,
            CreatedAt = CreatedAt,
            IsDeleted = IsDeleted,
            IsActive = IsActive,
            UserType = UserType,
        };

    public static Customer Reconstitute(UserSnapshot snapshot)
    {
        var c = new Customer
        {
            Id = snapshot.Id,
            Name = snapshot.Name,
            Email = snapshot.Email,
            CreatedAt = snapshot.CreatedAt,
        };
        c.RestorePassword(snapshot.PasswordHash, snapshot.PasswordSalt);
        c.RestoreIsDeleted(snapshot.IsDeleted);
        c.RestoreIsActive(snapshot.IsActive);
        c.RestoreUserType(snapshot.UserType);
        return c;
    }

    public bool HasActiveBorrowForBook(int bookId) =>
        Borrows.Any(b => b.BookId == bookId && !b.IsReturned);
}
