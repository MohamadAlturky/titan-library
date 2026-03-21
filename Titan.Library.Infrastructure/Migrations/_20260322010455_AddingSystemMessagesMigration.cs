using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class _20260322010455_AddingSystemMessagesMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS messages (
                id         SERIAL       PRIMARY KEY,
                key        VARCHAR(255) NOT NULL UNIQUE,
                value      TEXT         NOT NULL,
                created_at TIMESTAMPTZ  NOT NULL DEFAULT NOW()
            );
            """;

        await command.ExecuteNonQueryAsync();
    }
}
