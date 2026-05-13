using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1776527197, description: "Create users table")]
public sealed class CreateUsersMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        CREATE TABLE IF NOT EXISTS users (
            id BIGSERIAL PRIMARY KEY,
            authorization_id UUID UNIQUE NOT NULL,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        DROP TABLE IF EXISTS users
        """;
}