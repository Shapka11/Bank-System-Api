#pragma warning disable SA1649

using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1773413244, description: "Create accounts table")]
public sealed class CreateAccountsMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        CREATE TABLE IF NOT EXISTS accounts (
            id UUID PRIMARY KEY,
            account_number TEXT UNIQUE NOT NULL,
            password TEXT NOT NULL,
            balance DECIMAL NOT NULL,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL,
            updated_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        DROP TABLE IF EXISTS accounts
        """;
}