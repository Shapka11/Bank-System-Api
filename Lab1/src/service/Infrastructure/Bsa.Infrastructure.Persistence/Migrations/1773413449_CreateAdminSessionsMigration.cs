#pragma warning disable SA1649

using FluentMigrator;

namespace Bsa.Infrastructure.Persistence.Migrations;

[Migration(version: 1773413449, description: "Create admin sessions table")]
public class CreateAdminSessionsMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("""
        CREATE TABLE IF NOT EXISTS admin_sessions (
            id UUID PRIMARY KEY,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """);
    }

    public override void Down() => Execute.Sql("""DROP TABLE IF EXISTS admin_sessions""");
}