using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;
using C = Titan.Library.Infrastructure.Configurations.BookTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.BookTableConfiguration;

namespace Titan.Library.Infrastructure.Migrations;

public class M002_FixBookTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            DROP TABLE IF EXISTS {T.Table} CASCADE;
            CREATE TABLE IF NOT EXISTS {T.Table} (
                {C.Id}        SERIAL PRIMARY KEY,
                {C.Isbn}      VARCHAR(20) NOT NULL UNIQUE,
                {C.AuthorId}  INT NOT NULL,
                {C.Title}     TEXT NOT NULL,
                {C.CreatedAt} TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );
            CREATE INDEX IF NOT EXISTS idx_books_isbn   ON {T.Table}({C.Isbn});
            CREATE INDEX IF NOT EXISTS idx_books_title  ON {T.Table}({C.Title});
            CREATE INDEX IF NOT EXISTS idx_books_author ON {T.Table}({C.AuthorId});
            """;

        await command.ExecuteNonQueryAsync();
    }
}
