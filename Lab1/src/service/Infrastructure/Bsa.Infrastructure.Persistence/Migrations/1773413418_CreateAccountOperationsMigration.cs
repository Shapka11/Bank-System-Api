#pragma warning disable SA1649

using FluentMigrator;

namespace Bsa.Infrastructure.Persistence.Migrations;

[Migration(version: 1773413418, description: "Create account operations table")]
public class CreateAccountOperationsMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("""
        CREATE TABLE IF NOT EXISTS account_operations (
             id BIGSERIAL PRIMARY KEY,
             account_id BIGINT NOT NULL,
             account_number TEXT NOT NULL,
             balance DECIMAL NOT NULL,
             operation_type TEXT NOT NULL,
             created_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """);
    }

    public override void Down() => Execute.Sql("""DROP TABLE IF EXISTS account_operations""");
}