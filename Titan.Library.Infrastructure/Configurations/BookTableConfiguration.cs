namespace Titan.Library.Infrastructure.Configurations;

public static class BookTableConfiguration
{
    public const string Table = "books";

    public static class Columns
    {
        public const string Id = "id";
        public const string Isbn = "isbn";
        public const string AuthorId = "author_id";
        public const string Title = "title";
        public const string CreatedAt = "created_at";
        public const string IsAvailable = "is_available";
        public const string IsDeleted = "is_deleted";
    }
}
