using BankSystemApi.Application.Abstractions.Events.Models;
using Google.Type;
using Itmo.Dev.Platform.Kafka.Producer;

namespace BankSystemApi.Infrastructure.Kafka.Mapping;

public static class InvoiceMessageMappingExtensions
{
    public static KafkaProducerMessage<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue> ToMessage(
        this CreationInvoiceEvent evt)
    {
        var key = new ProtoInvoiceCreationKey
        {
            InvoiceId = evt.InvoiceId,
        };

        var value = new ProtoInvoiceCreationValue
        {
            InvoiceId = evt.InvoiceId,
            PayerId = evt.ReceiverAccountId,
            RecipientId = evt.SenderAccountId,
            Payment = new Money { DecimalValue = evt.Amount },
        };

        return new KafkaProducerMessage<ProtoInvoiceCreationKey, ProtoInvoiceCreationValue>(
            Key: key,
            Value: value);
    }
}