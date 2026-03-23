using Titan.Library.Domain.Borrows;

namespace Titan.Library.Domain.Users;

public class Customer : User
{
    // public Customer() => RestoreUserType(UserType.Customer);
    private Customer()
        : base()
    {
        RestoreUserType(UserType.Customer);
    }

    private readonly List<Borrow> _borrows = [];
    public IReadOnlyCollection<Borrow> Borrows => _borrows.AsReadOnly();

    public void AddBorrow(Borrow borrow) => _borrows.Add(borrow);

    public void ClearBorrows() => _borrows.Clear();

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

    public static Customer Create(string name, string email, string password)
    {
        var c = new Customer
        {
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow,
        };
        c.SetPassword(password);
        return c;
    }

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
