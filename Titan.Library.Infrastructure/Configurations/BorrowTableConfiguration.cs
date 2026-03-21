namespace Titan.Library.Infrastructure.Configurations;

public static class BorrowTableConfiguration
{
    public const string Table = "borrows";

    public static class Columns
    {
        public const string Id = "id";
        public const string CustomerId = "customer_id";
        public const string BookId = "book_id";
        public const string IsReturned = "is_returned";
        public const string ReturnedAt = "returned_at";
        public const string CreatedAt = "created_at";
    }
}
