using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1776075677, description: "Make user_id column not null in accounts table")]
public sealed class MakeUserIdNotNullInAccountsMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE accounts 
        ALTER COLUMN user_id SET NOT NULL
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE accounts 
        ALTER COLUMN user_id DROP NOT NULL
        """;
}