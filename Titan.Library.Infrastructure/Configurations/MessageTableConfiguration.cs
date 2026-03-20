namespace Titan.Library.Infrastructure.Configurations;

public static class MessageTableConfiguration
{
    public const string Table = "messages";

    public static class Columns
    {
        public const string Id = "id";
        public const string Key = "key";
        public const string Value = "value";
        public const string CreatedAt = "created_at";
    }
}
