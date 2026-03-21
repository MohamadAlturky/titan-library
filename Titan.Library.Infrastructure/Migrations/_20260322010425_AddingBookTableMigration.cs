using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class _20260322010425_AddingBookTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS books (
                id           SERIAL       PRIMARY KEY,
                isbn         VARCHAR(20)  NOT NULL UNIQUE,
                author_id    INT          NOT NULL REFERENCES authors(user_id) ON DELETE RESTRICT,
                title        VARCHAR(512) NOT NULL,
                created_at   TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
                is_available BOOLEAN      NOT NULL DEFAULT TRUE
            );
            """;

        await command.ExecuteNonQueryAsync();
    }
}
