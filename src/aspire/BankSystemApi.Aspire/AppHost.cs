using DotNetEnv;

Env.Load("../../../.env");

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

int prometheusPort = int.Parse(builder.Configuration["PROMETHEUS_PORT"] ?? "9090");
IResourceBuilder<ContainerResource> prometheus = builder
    .AddContainer("bank-system-prometheus", "prom/prometheus")
    .WithBindMount("../../../prometheus/prometheus.yml", "/etc/prometheus/prometheus.yml")
    .WithHttpEndpoint(port: prometheusPort, targetPort: prometheusPort, name: "prometheus-ui");

int grafanaPort = int.Parse(builder.Configuration["GRAFANA_PORT"] ?? "3000");
IResourceBuilder<ContainerResource> grafana = builder
    .AddContainer("bank-system-grafana", "grafana/grafana:latest")
    .WithHttpEndpoint(port: grafanaPort, targetPort: grafanaPort, name: "grafana-ui")
    .WithEnvironment("PROMETHEUS_URL", $"http://{prometheus.Resource.Name}:{prometheusPort.ToString()}")
    .WithBindMount("../../../grafana/provisioning", "/etc/grafana/provisioning")
    .WaitFor(prometheus);

IResourceBuilder<KeycloakResource> keycloak = builder
    .AddKeycloak("bank-keycloak")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter();

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("bank-service-postgres")
    .WithArgs("postgres", "-c", "max_prepared_transactions=100")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("bank-service-postgres-volume");

IResourceBuilder<PostgresDatabaseResource> bankServiceDatabase = postgres.AddDatabase("bank-postgres");
IResourceBuilder<PostgresDatabaseResource> approvalServiceDatabase = postgres.AddDatabase("approval-service-postgres");

IResourceBuilder<RedisResource> redis = builder.AddRedis("bank-system-redis");

IResourceBuilder<ContainerResource> zookeeper = builder
    .AddContainer("zookeeper", "wurstmeister/zookeeper", "latest")
    .WithEndpoint(port: 2181, targetPort: 2181, name: "tcp")
    .WithEnvironment("ALLOW_ANONYMOUS_LOGIN", "yes")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithVolume("bank-system-zookeeper-data", "/data")
    .WithVolume("bank-system-zookeeper-datalog", "/datalog");

IResourceBuilder<ContainerResource> kafka = builder
    .AddContainer("kafka", "wurstmeister/kafka", "latest")
    .WithEndpoint(port: 8001, targetPort: 9092, name: "tcp")
    .WithEnvironment("KAFKA_LOG_DIRS", "/kafka-data")
    .WithEnvironment("KAFKA_BROKER_ID", "1")
    .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", "zookeeper:2181")
    .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "INTERNAL")
    .WithEnvironment("KAFKA_LISTENERS", "EXTERNAL://0.0.0.0:9092,INTERNAL://0.0.0.0:9094")
    .WithEnvironment("KAFKA_ADVERTISED_LISTENERS", "EXTERNAL://127.0.0.1:8001,INTERNAL://kafka:9094")
    .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "EXTERNAL:PLAINTEXT,INTERNAL:PLAINTEXT")
    .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
    .WithEnvironment("KAFKA_AUTO_CREATE_TOPICS_ENABLE", "true")
    .WithEnvironment("ALLOW_PLAINTEXT_LISTENER", "yes")
    .WithEnvironment("KAFKA_CREATE_TOPICS", "account_created:1:1,invoice_created:1:1,approval_result:1:1")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithVolume("bank-system-kafka-data", "/kafka-data")
    .WaitFor(zookeeper);

IResourceBuilder<ContainerResource> kafkaUi = builder
    .AddContainer("kafka-ui", "provectuslabs/kafka-ui", "latest")
    .WithHttpEndpoint(port: 8003, targetPort: 8080, name: "http")
    .WaitFor(kafka)
    .WithEnvironment("SERVER_MAX_HTTP_HEADER_SIZE", "65536")
    .WithBindMount("../../service/Presentation/BankSystemApi.Presentation.Kafka/protos", "/schemas/protos", isReadOnly: true)
    .WithBindMount("../../service/Infrastructure/BankSystemApi.Infrastructure.Kafka/protos", "/schemas/infra-protos", isReadOnly: true)
    .WithEnvironment("kafka.clusters.0.name", "kafka")
    .WithEnvironment("kafka.clusters.0.bootstrapServers", "kafka:9094")
    .WithEnvironment("kafka.clusters.0.defaultKeySerde", "ProtobufFile")
    .WithEnvironment("kafka.clusters.0.defaultValueSerde", "ProtobufFile")
    .WithEnvironment("kafka.clusters.0.serde.0.name", "ProtobufFile")
    .WithEnvironment("kafka.clusters.0.serde.0.properties.protobufFilesDir", "/schemas/")
    .WithEnvironment("kafka.clusters.0.serde.0.properties.protobufMessageNameForKeyByTopic.account_created", "accounts.AccountCreationKey")
    .WithEnvironment("kafka.clusters.0.serde.0.properties.protobufMessageNameForKeyByTopic.invoice_created", "invoices.InvoiceCreationKey")
    .WithEnvironment("kafka.clusters.0.serde.0.properties.protobufMessageNameForKeyByTopic.approval_result", "approvals.ApprovalResultKey")
    .WithEnvironment("kafka.clusters.0.serde.0.properties.protobufMessageNameByTopic.account_created", "accounts.AccountCreationValue")
    .WithEnvironment("kafka.clusters.0.serde.0.properties.protobufMessageNameByTopic.invoice_created", "invoices.InvoiceCreationValue")
    .WithEnvironment("kafka.clusters.0.serde.0.properties.protobufMessageNameByTopic.approval_result", "approvals.ApprovalResultValue");

IResourceBuilder<ProjectResource> bankService = builder
    .AddProject<Projects.BankSystemApi>("bank-system-api-service")
    .WaitFor(bankServiceDatabase)
    .WaitFor(kafka)
    .WithEnvironment("Infrastructure__Persistence__Postgres__Host", postgres.Resource.PrimaryEndpoint.Property(EndpointProperty.Host))
    .WithEnvironment("Infrastructure__Persistence__Postgres__Port", postgres.Resource.PrimaryEndpoint.Property(EndpointProperty.Port))
    .WithEnvironment("Infrastructure__Persistence__Postgres__Database", bankServiceDatabase.Resource.DatabaseName)
    .WithEnvironment("Infrastructure__Persistence__Postgres__Username", postgres.Resource.UserNameReference)
    .WithEnvironment("Infrastructure__Persistence__Postgres__Password", postgres.Resource.PasswordParameter)
    .WithEnvironment("Presentation__Kafka__Host", "127.0.0.1:8001")
    .WithEnvironment("USE_PROMETHEUS_METRICS", builder.Configuration["USE_PROMETHEUS_METRICS"])
    .WithHttpHealthCheck("/health");

IResourceBuilder<ContainerResource> approvalService = builder
    .AddContainer("approval-service", "ghcr.io/ait-csbe-y28/lab-5-tools", "master")
    .WithContainerRuntimeArgs("--platform", "linux/amd64")
    .WithHttpEndpoint(port: 8070, targetPort: 8070, name: "grpc", isProxied: false)
    .WaitFor(approvalServiceDatabase)
    .WaitFor(kafka)
    .WaitFor(bankService)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8070")
    .WithEnvironment("Infrastructure__Persistence__Postgres__Host", postgres.Resource.Name)
    .WithEnvironment("Infrastructure__Persistence__Postgres__Port", "5432")
    .WithEnvironment("Infrastructure__Persistence__Postgres__Database", approvalServiceDatabase.Resource.DatabaseName)
    .WithEnvironment("Infrastructure__Persistence__Postgres__Username", postgres.Resource.UserNameReference)
    .WithEnvironment("Infrastructure__Persistence__Postgres__Password", postgres.Resource.PasswordParameter)
    .WithEnvironment("Presentation__Kafka__Host", "kafka:9094")
    .WithEnvironment("ServiceUrl__BaseUrl", bankService.GetEndpoint("gRPC"));

IResourceBuilder<ProjectResource> gateway = builder
    .AddProject<Projects.BankSystemApi_Gateway>("bank-system-api-gateway")
    .WaitFor(bankService)
    .WaitFor(keycloak)
    .WaitFor(redis)
    .WaitFor(approvalService)
    .WithEnvironment("Infrastructure__Clients__BankServiceClients__service-admin__BaseAddress", bankService.GetEndpoint("gRPC"))
    .WithEnvironment("Infrastructure__Clients__BankServiceClients__service-user__BaseAddress", bankService.GetEndpoint("gRPC"))
    .WithEnvironment("Infrastructure__Clients__BankServiceClients__service-invoice__BaseAddress", bankService.GetEndpoint("gRPC"))
    .WithEnvironment("Infrastructure__Clients__BankServiceClients__service-account__BaseAddress", bankService.GetEndpoint("gRPC"))
    .WithEnvironment("Infrastructure__Clients__BankServiceClients__service-history__BaseAddress", bankService.GetEndpoint("gRPC"))
    .WithEnvironment("Infrastructure__Clients__ApprovalServiceClients__service-invoice-approval__BaseAddress", approvalService.GetEndpoint("grpc"))
    .WithEnvironment("Infrastructure__Caching__Redis__ConnectionString", redis.Resource.ConnectionStringExpression)
    .WithEnvironment("Authentication__IdentityProviderUri", () => $"{keycloak.GetEndpoint("http").Url}/realms/master")
    .WithEnvironment("Authentication__ClientId", builder.Configuration["AUTH_CLIENT_ID"])
    .WithEnvironment("Authentication__ClientSecret", builder.Configuration["AUTH_CLIENT_SECRET"]);

builder.Build().Run();