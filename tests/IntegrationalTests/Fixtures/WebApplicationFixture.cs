using AutoBogus;
using BankSystemApi.Domain.ValueObjects;
using Grpc.Net.Client;
using Itmo.Dev.Platform.Testing.ApplicationFactories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace IntegrationalTests.Fixtures;

public sealed class WebApplicationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:latest").Build();

    private WebApplicationFactory<Program>? _webApplicationFactory;
    private Respawner? _respawner;

    public WebApplicationFactory<Program> Factory =>
        _webApplicationFactory ??
        throw new InvalidOperationException("The factory has not yet been initialized. Wait for InitializeAsync().");

    public IServiceProvider Services => Factory.Services;

    private static readonly string[] RespawnOptions = new[] { "public" };

    public async Task InitializeAsync()
    {
        InstallFakerConfig();

        Bogus.DataSets.Date.SystemClock = () => DateTime.UtcNow;
        await _container.StartAsync();

        _webApplicationFactory = new PlatformWebApplicationBuilder<Program>()
            .ConfigureConfiguration(builder => builder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Infrastructure:Persistence:Postgres:Host", _container.Hostname },
                {
                    "Infrastructure:Persistence:Postgres:Port",
                    _container.GetMappedPublicPort(5432).ToString()
                },
                { "Infrastructure:Persistence:Postgres:Database", "postgres" },
                { "Infrastructure:Persistence:Postgres:Username", "postgres" },
                { "Infrastructure:Persistence:Postgres:Password", "postgres" },
                { "Infrastructure:Persistence:Postgres:SslMode", "Disable" },
            }))
            .Build();

        _webApplicationFactory.StartServer();

        await ReloadNpgsqlTypesAsync(_webApplicationFactory);
        await InitializeRespawnerAsync();
    }

    public async Task DisposeAsync()
    {
        if (_webApplicationFactory is not null)
        {
            await _webApplicationFactory.DisposeAsync();
        }

        await _container.DisposeAsync();
    }

    public GrpcChannel CreateChannel()
    {
        var grpcChannelOptions = new GrpcChannelOptions
        {
            HttpHandler = Factory.Server.CreateHandler(),
        };

        return GrpcChannel.ForAddress("http://localhost", grpcChannelOptions);
    }

    public async Task ResetDatabaseAsync()
    {
        if (_respawner is not null)
        {
            await using var connection = new NpgsqlConnection(_container.GetConnectionString());
            await connection.OpenAsync();
            await _respawner.ResetAsync(connection);

            const string resetSequencesSql = """
            DO $$ 
            DECLARE 
                r RECORD;
            BEGIN 
                FOR r IN (SELECT sequence_name FROM information_schema.sequences WHERE sequence_schema = 'public') 
                LOOP 
                    EXECUTE 'ALTER SEQUENCE ' || quote_ident(r.sequence_name) || ' RESTART WITH 1';
                END LOOP; 
            END $$;
            """;

            await using var command = new NpgsqlCommand(resetSequencesSql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    private async Task InitializeRespawnerAsync()
    {
        await using var connection = new NpgsqlConnection(_container.GetConnectionString());
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = RespawnOptions,
        });
    }

    private void InstallFakerConfig()
    {
        AutoFaker.Configure(builder =>
        {
            builder.WithOverride<Money>(faker => new Money(faker.Faker.Finance.Amount()));
        });
    }

    private async Task ReloadNpgsqlTypesAsync(WebApplicationFactory<Program> webApplicationFactory)
    {
        NpgsqlDataSource? dataSource = webApplicationFactory.Services.GetService<NpgsqlDataSource>();
        if (dataSource is not null)
        {
            await dataSource.ReloadTypesAsync();
        }
        else
        {
            await using var connection = new NpgsqlConnection(_container.GetConnectionString());
            await connection.OpenAsync();
            await connection.ReloadTypesAsync();
        }
    }
}