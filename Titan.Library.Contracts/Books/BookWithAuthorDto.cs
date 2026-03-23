using Titan.Library.Domain.Books;

namespace Titan.Library.Contracts.Books;

public class BookWithAuthorDto
{
    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsAvailable { get; set; }

    public static BookWithAuthorDto FromEntity(BookWithAuthor entity) =>
        new()
        {
            Id = entity.Book.Id,
            Isbn = entity.Book.Isbn,
            Title = entity.Book.Title,
            Description = entity.Book.Description,
            AuthorId = entity.Book.AuthorId,
            AuthorName = entity.AuthorName,
            AuthorEmail = entity.AuthorEmail,
            CreatedAt = entity.Book.CreatedAt,
            IsAvailable = entity.Book.IsAvailable,
        };
}
