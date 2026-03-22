namespace Titan.Library.Domain.Books;

public sealed class BookSnapshot
{
    public int Id { get; init; }
    public string Isbn { get; init; } = string.Empty;
    public int AuthorId { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsAvailable { get; init; }
    public bool IsDeleted { get; init; }
}
