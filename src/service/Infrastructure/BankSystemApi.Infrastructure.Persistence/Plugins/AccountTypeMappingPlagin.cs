using BankSystemApi.Domain.Accounts;
using Itmo.Dev.Platform.Persistence.Postgres.Plugins;
using Npgsql;
using Npgsql.NameTranslation;

namespace BankSystemApi.Infrastructure.Persistence.Plugins;

public sealed class AccountTypeMappingPlagin : IPostgresDataSourcePlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSource)
    {
        dataSource.MapEnum<AccountType>("account_type", new NpgsqlSnakeCaseNameTranslator());
    }
}