#pragma warning disable SA1649

using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Bsa.Infrastructure.Persistence.Migrations;

[Migration(version: 1773413418, description: "Create account operations table")]
public sealed class CreateHistoryOperationsMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        CREATE TABLE IF NOT EXISTS history_operations (
             id BIGSERIAL PRIMARY KEY,
             account_id BIGINT NOT NULL,
             account_number TEXT NOT NULL,
             type TEXT NOT NULL,
             amount DECIMAL NOT NULL,
             occurred_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        DROP TABLE IF EXISTS history_operations
        """;
}