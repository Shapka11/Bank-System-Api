using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.ValueObjects;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Invoices;

public sealed class InvoicePaymentReceivedHistoryOperationChain : HistoryOperationSerializationChain
{
    public override SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
    {
        if (operation is not InvoicePaymentReceivedHistoryOperation invoicePaymentReceivedOperation)
            return NextChainElement.SerializeSingle(operation);

        var payload = new InvoicePaymentReceivedPayload(
            invoicePaymentReceivedOperation.Amount.Value,
            invoicePaymentReceivedOperation.InvoiceId.Value);
        string payloadJson = JsonSerializer.Serialize<PayloadBase>(payload);

        return new SerializationHistoryOperationResult.Success([payloadJson]);
    }

    public override DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
    {
        if (historyEntry.Payload is not InvoicePaymentReceivedPayload paymentReceivedPayload)
            return NextChainElement.Deserialize(historyEntry);

        var invoicePaymentReceived = new InvoicePaymentReceivedHistoryOperation(
            historyEntry.HistoryOperationId,
            historyEntry.AccountId,
            new Money(paymentReceivedPayload.Amount),
            new InvoiceId(paymentReceivedPayload.InvoiceId),
            historyEntry.CreatedAt);

        return new DeserializationHistoryOperationResult.Success(invoicePaymentReceived);
    }
}