#pragma warning disable SA1649

using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1776527018, description: "drop sessions table")]
public sealed class DropSessionsMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        DROP TABLE IF EXISTS sessions
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        CREATE TABLE IF NOT EXISTS sessions (
            session_id UUID PRIMARY KEY,
            account_id BIGINT,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """;
}