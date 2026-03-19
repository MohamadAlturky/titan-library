using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class FixBookTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    public override int Order() => 2;

    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = """
            DROP TABLE IF EXISTS books CASCADE;
            CREATE TABLE IF NOT EXISTS books (
                id         SERIAL PRIMARY KEY,
                isbn       VARCHAR(20) NOT NULL UNIQUE,
                author_id  INT NOT NULL,
                title      TEXT NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );
            CREATE INDEX IF NOT EXISTS idx_books_isbn   ON books(isbn);
            CREATE INDEX IF NOT EXISTS idx_books_title  ON books(title);
            CREATE INDEX IF NOT EXISTS idx_books_author ON books(author_id);
            """;

        await command.ExecuteNonQueryAsync();
    }
}
