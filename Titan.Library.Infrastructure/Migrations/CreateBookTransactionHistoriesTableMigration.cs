using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class CreateBookTransactionHistoriesTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    public override int Order() => 5;

    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = """
            CREATE TABLE IF NOT EXISTS book_quantity_transaction_histories (
                id               SERIAL PRIMARY KEY,
                book_id          INT NOT NULL REFERENCES books(id) ON DELETE CASCADE,
                amount           INT NOT NULL,
                transaction_type INT NOT NULL,
                created_at       TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );
            CREATE INDEX IF NOT EXISTS idx_bqth_book_id ON book_quantity_transaction_histories(book_id);
            """;

        await command.ExecuteNonQueryAsync();
    }
}
