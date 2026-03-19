using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class CreateBorrowsTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    public override int Order() => 4;

    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = """
            CREATE TABLE IF NOT EXISTS borrows (
                id          SERIAL PRIMARY KEY,
                customer_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                book_id     INT NOT NULL REFERENCES books(id) ON DELETE CASCADE,
                borrowed_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
                returned_at TIMESTAMP WITH TIME ZONE NULL,
                created_at  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );
            CREATE INDEX IF NOT EXISTS idx_borrows_customer_id ON borrows(customer_id);
            CREATE INDEX IF NOT EXISTS idx_borrows_book_id     ON borrows(book_id);
            """;

        await command.ExecuteNonQueryAsync();
    }
}
