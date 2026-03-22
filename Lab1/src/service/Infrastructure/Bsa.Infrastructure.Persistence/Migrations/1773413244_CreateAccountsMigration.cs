#pragma warning disable SA1649

using FluentMigrator;

namespace Bsa.Infrastructure.Persistence.Migrations;

[Migration(version: 1773413244, description: "Create accounts table")]
public class CreateAccountsMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("""
        CREATE TABLE IF NOT EXISTS accounts (
            id BIGSERIAL PRIMARY KEY,
            account_number TEXT UNIQUE NOT NULL,
            password TEXT NOT NULL,
            balance DECIMAL NOT NULL,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL,
            updated_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """);
    }

    public override void Down() => Execute.Sql("""DROP TABLE IF EXISTS accounts""");
}