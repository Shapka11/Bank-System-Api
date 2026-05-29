using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1774513005, description: "Remove type column from history_operations table")]
public sealed class RemoveTypeFromHistoryOperationsMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE history_operations 
        DROP COLUMN IF EXISTS type
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE history_operations 
        ADD COLUMN IF NOT EXISTS type TEXT;

        UPDATE history_operations
        SET type = payload->>'type'
        WHERE payload ? 'type';

        ALTER TABLE history_operations 
        ALTER COLUMN type SET NOT NULL;
        """;
}