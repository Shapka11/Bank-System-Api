using BankSystemApi.Presentation.Kafka.Handlers;
using Itmo.Dev.Platform.Kafka.Configuration;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;

namespace BankSystemApi.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IKafkaConfigurationBuilder AddPresentationKafkaConsumers(
        this IKafkaConfigurationBuilder kafka,
        IConfiguration configuration)
    {
        configuration = configuration.GetSection("Presentation:Kafka:Consumers");

        kafka.AddConsumer(b => b
            .WithKey<ProtoApprovalResultKey>()
            .WithValue<ProtoApprovalResultValue>()
            .WithConfiguration(configuration.GetSection("ApprovalResult"))
            .DeserializeKeyWithProto()
            .DeserializeValueWithProto()
            .HandleWith<ApprovalResultKafkaHandler>());

        return kafka;
    }
}