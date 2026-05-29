using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1779531613, description: "Make type column not null in accounts table")]
public sealed class MakeAccountTypeNotNullMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE accounts 
        ALTER COLUMN type SET NOT NULL
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE accounts 
        ALTER COLUMN type DROP NOT NULL
        """;
}