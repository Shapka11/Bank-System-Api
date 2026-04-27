using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1773820091, description: "Add jsonb payload column as nullable to account operations")]
public sealed class AddPayloadNullableMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE history_operations 
        ADD COLUMN IF NOT EXISTS payload JSONB
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE history_operations 
        DROP COLUMN IF EXISTS payload
        """;
}