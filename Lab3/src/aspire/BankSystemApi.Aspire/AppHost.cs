using DotNetEnv;

Env.Load("../../../.env");

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ContainerResource> prometheus = builder
    .AddContainer("bank-system-prometheus", "prom/prometheus")
    .WithBindMount("../../../prometheus/prometheus.yml", "/etc/prometheus/prometheus.yml")
    .WithHttpEndpoint(port: 9090, targetPort: 9090, name: "prometheus-ui");

IResourceBuilder<KeycloakResource> keycloak = builder
    .AddKeycloak("bank-keycloak")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter();

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("bank-service-postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("bank-service-postgres-volume");

IResourceBuilder<PostgresDatabaseResource> database = postgres.AddDatabase("postgres");

IResourceBuilder<RedisResource> redis = builder.AddRedis("bank-system-redis");

IResourceBuilder<ProjectResource> service = builder
    .AddProject<Projects.BankSystemApi>("bank-system-api-service")
    .WaitFor(database)
    .WithEnvironment(
        "Infrastructure:Persistence:Postgres:Host",
        postgres.Resource.PrimaryEndpoint.Property(EndpointProperty.Host))
    .WithEnvironment(
        "Infrastructure:Persistence:Postgres:Port",
        postgres.Resource.PrimaryEndpoint.Property(EndpointProperty.Port))
    .WithEnvironment(
        "Infrastructure:Persistence:Postgres:Database",
        database.Resource.DatabaseName)
    .WithEnvironment(
        "Infrastructure:Persistence:Postgres:Username",
        postgres.Resource.UserNameReference)
    .WithEnvironment(
        "Infrastructure:Persistence:Postgres:Password",
        postgres.Resource.PasswordParameter)
    .WithEnvironment("USE_PROMETHEUS_METRICS", "true")
    .WithHttpHealthCheck("/health");

IResourceBuilder<ProjectResource> gateway = builder
    .AddProject<Projects.BankSystemApi_Gateway>("bank-system-api-gateway")
    .WaitFor(service)
    .WaitFor(keycloak)
    .WaitFor(redis)
    .WithEnvironment(
        "Infrastructure:Clients:BankServiceClients:service-admin:BaseAddress",
        service.GetEndpoint("gRPC"))
    .WithEnvironment(
        "Infrastructure:Clients:BankServiceClients:service-user:BaseAddress",
        service.GetEndpoint("gRPC"))
    .WithEnvironment(
        "Infrastructure:Clients:BankServiceClients:service-invoice:BaseAddress",
        service.GetEndpoint("gRPC"))
    .WithEnvironment(
        "Infrastructure:Clients:BankServiceClients:service-account:BaseAddress",
        service.GetEndpoint("gRPC"))
    .WithEnvironment(
        "Infrastructure:Clients:BankServiceClients:service-history:BaseAddress",
        service.GetEndpoint("gRPC"))
    .WithEnvironment(
        "Infrastructure:Caching:Redis:ConnectionString",
        redis.Resource.ConnectionStringExpression)
    .WithEnvironment(
        "Authentication__IdentityProviderUri",
        () => $"{keycloak.GetEndpoint("http").Url}/realms/master")
    .WithEnvironment(
        "Authentication__ClientId",
        builder.Configuration["AUTH_CLIENT_ID"])
    .WithEnvironment(
        "Authentication__ClientSecret",
        builder.Configuration["AUTH_CLIENT_SECRET"]);

builder.Build().Run();