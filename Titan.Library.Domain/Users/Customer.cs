using Titan.Library.Domain.Borrows;

namespace Titan.Library.Domain.Users;

public class Customer : User
{
    public List<Borrow> Borrows { get; set; } = [];
}
