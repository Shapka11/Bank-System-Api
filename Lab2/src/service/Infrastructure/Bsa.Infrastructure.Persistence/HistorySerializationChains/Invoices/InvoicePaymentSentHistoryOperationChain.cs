using Bsa.Domain.HistoryOperations;
using Bsa.Domain.HistoryOperations.Invoices;
using Bsa.Domain.Invoices;
using Bsa.Domain.ValueObjects;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;
using Bsa.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Invoices;

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
            historyEntry.AccountNumber,
            new Money(paymentSentPayload.Amount),
            new InvoiceId(paymentSentPayload.InvoiceId),
            historyEntry.CreatedTime);

        return new DeserializationHistoryOperationResult.Success(deposit);
    }
}