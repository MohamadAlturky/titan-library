using Titan.Library.Domain.Books;

namespace Titan.Library.Domain.Users;

public class Author : User
{
    public List<Book> Books { get; set; } = [];
}
