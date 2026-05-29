using BankSystemApi.Application.Abstractions.Events.Models;
using BankSystemApi.Application.Abstractions.Events.Publishers;
using BankSystemApi.Infrastructure.Kafka.Mapping;
using Itmo.Dev.Platform.Kafka.Producer;

namespace BankSystemApi.Infrastructure.Kafka.Publishers;

internal sealed class AccountEventPublisher : IAccountEventPublisher
{
    private readonly IKafkaMessageProducer<ProtoAccountCreationKey, ProtoAccountCreationValue> _producer;

    public AccountEventPublisher(IKafkaMessageProducer<ProtoAccountCreationKey, ProtoAccountCreationValue> producer)
    {
        _producer = producer;
    }

    public async Task Publish(
        IReadOnlyList<CreationAccountEvent> creationAccountEvents,
        CancellationToken cancellationToken)
    {
        IAsyncEnumerable<KafkaProducerMessage<ProtoAccountCreationKey, ProtoAccountCreationValue>> messages =
            creationAccountEvents
                .Select(evt => evt.ToMessage())
                .ToAsyncEnumerable();

        await _producer.ProduceAsync(messages, cancellationToken);
    }
}