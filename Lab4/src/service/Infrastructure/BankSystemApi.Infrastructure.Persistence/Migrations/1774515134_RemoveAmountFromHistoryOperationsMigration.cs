using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1774515134, description: "Remove amount column from history_operations table")]
public sealed class RemoveAmountFromHistoryOperationsMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE history_operations 
        DROP COLUMN IF EXISTS amount
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE history_operations 
        ADD COLUMN IF NOT EXISTS amount DECIMAL;

        UPDATE history_operations
        SET amount = payload->>'amount'
        WHERE payload ? 'amount';

        ALTER TABLE history_operations 
        ALTER COLUMN amount SET NOT NULL;
        """;
}