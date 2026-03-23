namespace Titan.Library.Infrastructure.Configurations;

public static class FeedbackTableConfiguration
{
    public const string Table = "feedbacks";

    public static class Columns
    {
        public const string Id = "id";
        public const string CustomerId = "customer_id";
        public const string Category = "category";
        public const string Rating = "rating";
        public const string Subject = "subject";
        public const string Message = "message";
        public const string CreatedAt = "created_at";
    }
}
