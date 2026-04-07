using Bsa.Domain.Invoices.States;
using Itmo.Dev.Platform.Persistence.Postgres.Plugins;
using Npgsql;
using Npgsql.NameTranslation;

namespace Bsa.Infrastructure.Persistence.Plugins;

public sealed class InvoiceStatusMappingPlugin : IPostgresDataSourcePlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSource)
    {
        dataSource.MapEnum<InvoiceStatus>("invoice_status", new NpgsqlNullNameTranslator());
    }
}