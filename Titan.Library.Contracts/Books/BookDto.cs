using Titan.Library.Common.Dtos;
using Titan.Library.Domain.Books;

namespace Titan.Library.Contracts.Books;

public class BookDto : BaseDto<Book, int>
{
    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }

    public override void Map(Book entity)
    {
        Id = entity.Id;
        Isbn = entity.Isbn;
        Title = entity.Title;
        AuthorId = entity.AuthorId;
        CreatedAt = entity.CreatedAt;
    }
}