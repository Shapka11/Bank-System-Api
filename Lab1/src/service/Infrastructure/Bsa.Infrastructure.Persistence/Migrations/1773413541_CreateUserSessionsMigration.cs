#pragma warning disable SA1649

using FluentMigrator;

namespace Bsa.Infrastructure.Persistence.Migrations;

[Migration(version: 1773413541, description: "Create user sessions table")]
public class CreateUserSessionsMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("""
        CREATE TABLE IF NOT EXISTS user_sessions (
            id UUID PRIMARY KEY,
            account_id BIGINT NOT NULL,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """);
    }

    public override void Down() => Execute.Sql("""DROP TABLE IF EXISTS user_sessions""");
}