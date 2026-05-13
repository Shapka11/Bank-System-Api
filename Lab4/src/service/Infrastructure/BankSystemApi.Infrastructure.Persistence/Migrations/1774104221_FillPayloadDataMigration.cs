#pragma warning disable SA1649

using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace BankSystemApi.Infrastructure.Persistence.Migrations;

[Migration(version: 1774104221, description: "Populate payload with initial data")]
public sealed class FillPayloadDataMigration : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
        """
        UPDATE history_operations
        SET payload = CASE
            WHEN type = 'CreateAccount' THEN json_build_object('$type', 'create_account')
            WHEN type = 'Deposit' THEN json_build_object('$type', 'deposit', 'amount', amount)
            WHEN type = 'Withdraw' THEN json_build_object('$type', 'withdraw', 'amount', amount)
            WHEN type = 'CheckBalance' THEN json_build_object('$type', 'check_balance', 'balance', amount)
            ELSE json_build_object('$type', type)
        END
        WHERE payload IS NULL;
        
        ALTER TABLE history_operations 
        ALTER COLUMN payload SET NOT NULL
        """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
        """
        ALTER TABLE history_operations 
        ALTER COLUMN payload DROP NOT NULL;        

        UPDATE history_operations SET payload = NULL
        """;
}