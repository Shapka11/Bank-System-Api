using Bsa.Domain.HistoryOperations;
using Bsa.Domain.HistoryOperations.Invoices;
using Bsa.Domain.Invoices;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;
using Bsa.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Invoices;

public sealed class InvoiceReceivedHistoryOperationChain : HistoryOperationSerializationChain
{
    public override SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
    {
        if (operation is not InvoiceReceivedHistoryOperation invoiceIssuedOperation)
            return NextChainElement.SerializeSingle(operation);

        var payload = new InvoiceReceivedPayload(invoiceIssuedOperation.InvoiceId.Value);
        string payloadJson = JsonSerializer.Serialize<PayloadBase>(payload);

        return new SerializationHistoryOperationResult.Success([payloadJson]);
    }

    public override DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
    {
        if (historyEntry.Payload is not InvoiceReceivedPayload receivedPayload)
            return NextChainElement.Deserialize(historyEntry);

        var deposit = new InvoiceReceivedHistoryOperation(
            historyEntry.HistoryOperationId,
            historyEntry.AccountId,
            historyEntry.AccountNumber,
            new InvoiceId(receivedPayload.InvoiceId),
            historyEntry.CreatedTime);

        return new DeserializationHistoryOperationResult.Success(deposit);
    }
}