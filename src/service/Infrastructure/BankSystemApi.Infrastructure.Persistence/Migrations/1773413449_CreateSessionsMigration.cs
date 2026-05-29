#pragma warning disable SA1649

using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1773413449, description: "Create sessions table")]
public sealed class CreateSessionsMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        CREATE TABLE IF NOT EXISTS sessions (
            session_id UUID PRIMARY KEY,
            account_id BIGINT,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        DROP TABLE IF EXISTS sessions
        """;
}