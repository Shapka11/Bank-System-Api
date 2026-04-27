using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1776075272, description: "Add guid user_id column as nullable to accounts")]
public sealed class AddUserIdFromAccountsMigrations : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE accounts 
        ADD COLUMN IF NOT EXISTS user_id BIGINT
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE accounts 
        DROP COLUMN IF EXISTS user_id
        """;
}