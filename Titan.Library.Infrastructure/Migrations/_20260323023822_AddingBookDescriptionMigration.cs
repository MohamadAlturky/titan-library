using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class _20260323023822_AddingBookDescriptionMigration(
    IDbConnectionFactory dbConnectionFactory
) : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            ALTER TABLE books
            ADD COLUMN description VARCHAR(1024);
            """;

        await command.ExecuteNonQueryAsync();
    }
}
