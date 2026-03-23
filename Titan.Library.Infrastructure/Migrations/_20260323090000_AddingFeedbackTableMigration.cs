using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class _20260323090000_AddingFeedbackTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS feedbacks (
                id          SERIAL       PRIMARY KEY,
                customer_id INT          NOT NULL REFERENCES users(id),
                category    VARCHAR(50)  NOT NULL DEFAULT 'general',
                rating      SMALLINT     CHECK (rating BETWEEN 1 AND 5),
                subject     VARCHAR(120) NOT NULL,
                message     TEXT         NOT NULL,
                created_at  TIMESTAMPTZ  NOT NULL DEFAULT NOW()
            );

            CREATE INDEX IF NOT EXISTS idx_feedbacks_customer_id ON feedbacks(customer_id);
            """;

        await command.ExecuteNonQueryAsync();
    }
}
