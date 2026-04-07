using Bsa.Domain.HistoryOperations;
using Bsa.Domain.HistoryOperations.Invoices;
using Bsa.Domain.Invoices;
using Bsa.Domain.ValueObjects;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;
using Bsa.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Invoices;

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
            historyEntry.AccountNumber,
            new Money(paymentReceivedPayload.Amount),
            new InvoiceId(paymentReceivedPayload.InvoiceId),
            historyEntry.CreatedTime);

        return new DeserializationHistoryOperationResult.Success(invoicePaymentReceived);
    }
}