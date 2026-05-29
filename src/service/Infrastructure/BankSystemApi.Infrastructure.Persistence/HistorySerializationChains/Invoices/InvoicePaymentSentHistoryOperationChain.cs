using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.ValueObjects;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Invoices;

public sealed class InvoicePaymentSentHistoryOperationChain : HistoryOperationSerializationChain
{
    public override SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
    {
        if (operation is not InvoicePaymentSentHistoryOperation invoicePaymentSentOperation)
            return NextChainElement.SerializeSingle(operation);

        var payload = new InvoicePaymentSentPayload(
            invoicePaymentSentOperation.Amount.Value,
            invoicePaymentSentOperation.InvoiceId.Value);
        string payloadJson = JsonSerializer.Serialize<PayloadBase>(payload);

        return new SerializationHistoryOperationResult.Success([payloadJson]);
    }

    public override DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
    {
        if (historyEntry.Payload is not InvoicePaymentSentPayload paymentSentPayload)
            return NextChainElement.Deserialize(historyEntry);

        var deposit = new InvoicePaymentSentHistoryOperation(
            historyEntry.HistoryOperationId,
            historyEntry.AccountId,
            new Money(paymentSentPayload.Amount),
            new InvoiceId(paymentSentPayload.InvoiceId),
            historyEntry.CreatedAt);

        return new DeserializationHistoryOperationResult.Success(deposit);
    }
}