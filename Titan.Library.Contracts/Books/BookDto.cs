using Titan.Library.Common.Dtos;
using Titan.Library.Domain.Books;

namespace Titan.Library.Contracts.Books;

public class BookDto : BaseDto<Book, int>
{
    public override void Map(Book entity)
    {
    }
}