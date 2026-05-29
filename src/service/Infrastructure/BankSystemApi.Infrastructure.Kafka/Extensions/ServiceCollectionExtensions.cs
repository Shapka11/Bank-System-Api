using BankSystemApi.Application.Abstractions.Events.Publishers;
using BankSystemApi.Infrastructure.Kafka.Publishers;
using Itmo.Dev.Platform.Kafka.Configuration;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankSystemApi.Infrastructure.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IKafkaConfigurationBuilder AddInfrastructureKafkaProducers(
        this IKafkaConfigurationBuilder kafka,
        IConfiguration configuration)
    {
        configuration = configuration.GetSection("Presentation:Kafka:Producers");

        kafka.AddProducer(b => b
            .WithKey<ProtoAccountCreationKey>()
            .WithValue<ProtoAccountCreationValue>()
            .WithConfiguration(configuration.GetSection("AccountCreated"))
            .SerializeKeyWithProto()
            .SerializeValueWithProto()
            .WithOutbox());

        kafka.AddProducer(b => b
            .WithKey<ProtoInvoiceCreationKey>()
            .WithValue<ProtoInvoiceCreationValue>()
            .WithConfiguration(configuration.GetSection("InvoiceCreated"))
            .SerializeKeyWithProto()
            .SerializeValueWithProto()
            .WithOutbox());

        return kafka;
    }

    public static IServiceCollection AddEventPublishers(this IServiceCollection collection)
    {
        collection.AddScoped<IInvoiceEventPublisher, InvoiceEventPublisher>();
        collection.AddScoped<IAccountEventPublisher, AccountEventPublisher>();

        return collection;
    }
}