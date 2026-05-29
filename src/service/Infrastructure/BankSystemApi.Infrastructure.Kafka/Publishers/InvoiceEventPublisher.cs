using BankSystemApi.Application.Abstractions.Events.Models;
using BankSystemApi.Application.Abstractions.Events.Publishers;
using BankSystemApi.Infrastructure.Kafka.Mapping;
using Itmo.Dev.Platform.Kafka.Producer;

namespace BankSystemApi.Infrastructure.Kafka.Publishers;

internal sealed class InvoiceEventPublisher : IInvoiceEventPublisher
{
    private readonly IKafkaMessageProducer<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue> _producer;

    public InvoiceEventPublisher(IKafkaMessageProducer<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue> producer)
    {
        _producer = producer;
    }

    public async Task Publish(
        IReadOnlyList<CreationInvoiceEvent> creationInvoiceEvents,
        CancellationToken cancellationToken)
    {
        IAsyncEnumerable<KafkaProducerMessage<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue>> messages =
            creationInvoiceEvents
                .Select(evt => evt.ToMessage())
                .ToAsyncEnumerable();

        await _producer.ProduceAsync(messages, cancellationToken);
    }
}