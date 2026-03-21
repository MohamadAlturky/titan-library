using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class _20260322010439_AddingBorrowMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS borrows (
                id          SERIAL      PRIMARY KEY,
                customer_id INT         NOT NULL REFERENCES customers(user_id) ON DELETE RESTRICT,
                book_id     INT         NOT NULL REFERENCES books(id) ON DELETE RESTRICT,
                is_returned BOOLEAN     NOT NULL DEFAULT FALSE,
                returned_at TIMESTAMPTZ NULL,
                created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
            );
            """;

        await command.ExecuteNonQueryAsync();
    }
}
