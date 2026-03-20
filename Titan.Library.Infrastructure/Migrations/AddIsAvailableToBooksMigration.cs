using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;
using C = Titan.Library.Infrastructure.Configurations.BookTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.BookTableConfiguration;

namespace Titan.Library.Infrastructure.Migrations;

public class AddIsAvailableToBooksMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    public override int Order() => 7;

    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            DROP TABLE IF EXISTS book_quantity_transaction_histories;
            ALTER TABLE {T.Table}
                ADD COLUMN IF NOT EXISTS {C.IsAvailable} BOOLEAN NOT NULL DEFAULT FALSE;
            """;

        await command.ExecuteNonQueryAsync();
    }
}
