using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;
using BC = Titan.Library.Infrastructure.Configurations.BookTableConfiguration.Columns;
using BT = Titan.Library.Infrastructure.Configurations.BookTableConfiguration;
using C = Titan.Library.Infrastructure.Configurations.BorrowTableConfiguration.Columns;
using CT = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.CustomerTable;
using T = Titan.Library.Infrastructure.Configurations.BorrowTableConfiguration;

namespace Titan.Library.Infrastructure.Migrations;

public class M005_CreateBorrowsTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS {T.Table} (
                {C.Id}         SERIAL PRIMARY KEY,
                {C.CustomerId} INT NOT NULL REFERENCES {CT.Table}({CT.UserId}) ON DELETE CASCADE,
                {C.BookId}     INT NOT NULL REFERENCES {BT.Table}({BC.Id}) ON DELETE CASCADE,
                {C.BorrowedAt} TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
                {C.ReturnedAt} TIMESTAMP WITH TIME ZONE NULL,
                {C.CreatedAt}  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );
            CREATE INDEX IF NOT EXISTS idx_borrows_customer_id ON {T.Table}({C.CustomerId});
            CREATE INDEX IF NOT EXISTS idx_borrows_book_id     ON {T.Table}({C.BookId});
            """;

        await command.ExecuteNonQueryAsync();
    }
}
