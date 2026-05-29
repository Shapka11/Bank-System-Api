using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1779531464, description: "Add type column as nullable to accounts")]
public sealed class AddAccountTypeMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        CREATE TYPE account_type AS ENUM ('personal', 'corporate');

        ALTER TABLE accounts 
        ADD COLUMN IF NOT EXISTS type account_type;
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE accounts 
        DROP COLUMN IF EXISTS type;

        DROP TYPE IF EXISTS account_type;
        """;
}