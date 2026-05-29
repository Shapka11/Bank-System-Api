#pragma warning disable SA1649

using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1773413418, description: "Create account operations table")]
public sealed class CreateHistoryOperationsMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        CREATE TABLE IF NOT EXISTS history_operations (
             history_operation_id BIGSERIAL PRIMARY KEY,
             account_id BIGINT NOT NULL,
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