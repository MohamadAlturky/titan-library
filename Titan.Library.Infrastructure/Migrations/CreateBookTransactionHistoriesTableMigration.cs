using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;
using BC = Titan.Library.Infrastructure.Configurations.BookTableConfiguration.Columns;
using BT = Titan.Library.Infrastructure.Configurations.BookTableConfiguration;
using C = Titan.Library.Infrastructure.Configurations.BookTransactionHistoryTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.BookTransactionHistoryTableConfiguration;

namespace Titan.Library.Infrastructure.Migrations;

public class CreateBookTransactionHistoriesTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    public override int Order() => 6;

    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS {T.Table} (
                {C.Id}              SERIAL PRIMARY KEY,
                {C.BookId}          INT NOT NULL REFERENCES {BT.Table}({BC.Id}) ON DELETE CASCADE,
                {C.Amount}          INT NOT NULL,
                {C.TransactionType} INT NOT NULL,
                {C.CreatedAt}       TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );
            CREATE INDEX IF NOT EXISTS idx_bqth_book_id ON {T.Table}({C.BookId});
            """;

        await command.ExecuteNonQueryAsync();
    }
}
