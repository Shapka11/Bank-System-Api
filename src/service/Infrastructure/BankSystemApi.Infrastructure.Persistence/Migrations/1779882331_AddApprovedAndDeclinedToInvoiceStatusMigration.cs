using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

#pragma warning disable SA1649

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1779531700, description: "Add approved and declined values to invoice_status enum")]
public sealed class AddApprovedAndDeclinedToInvoiceStatusMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        ALTER TYPE invoice_status ADD VALUE IF NOT EXISTS 'approved';
        ALTER TYPE invoice_status ADD VALUE IF NOT EXISTS 'declined';
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) => string.Empty;
}