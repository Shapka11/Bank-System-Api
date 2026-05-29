using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1774515193, description: "Create invoices table")]
public sealed class CreateInvoiceMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        CREATE TYPE invoice_status AS ENUM ('created', 'paid', 'revoked');

        CREATE TABLE IF NOT EXISTS invoices (
            invoice_id BIGSERIAL PRIMARY KEY,
            sender_account_id BIGINT NOT NULL,
            receiver_account_id BIGINT NOT NULL,
            amount DECIMAL NOT NULL,
            status invoice_status NOT NULL,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL,
            updated_at TIMESTAMP WITH TIME ZONE NOT NULL
        )
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        DROP TABLE IF EXISTS invoices
        """;
}