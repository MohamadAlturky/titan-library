namespace Titan.Library.Infrastructure.Configurations;

public static class BookTransactionHistoryTableConfiguration
{
    public const string Table = "book_quantity_transaction_histories";

    public static class Columns
    {
        public const string Id = "id";
        public const string BookId = "book_id";
        public const string Amount = "amount";
        public const string TransactionType = "transaction_type";
        public const string CreatedAt = "created_at";
    }
}
