using Bsa.Domain.HistoryOperations;
using Bsa.Domain.HistoryOperations.Invoices;
using Bsa.Domain.Invoices;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;
using Bsa.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Invoices;

public sealed class InvoiceIssuedHistoryOperationChain : HistoryOperationSerializationChain
{
    public override SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
    {
        if (operation is not InvoiceIssuedHistoryOperation invoiceIssuedOperation)
            return NextChainElement.SerializeSingle(operation);

        var payload = new InvoiceIssuedPayload(invoiceIssuedOperation.InvoiceId.Value);
        string payloadJson = JsonSerializer.Serialize<PayloadBase>(payload);

        return new SerializationHistoryOperationResult.Success([payloadJson]);
    }

    public override DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
    {
        if (historyEntry.Payload is not InvoiceIssuedPayload issuedPayload)
            return NextChainElement.Deserialize(historyEntry);

        var invoiceIssued = new InvoiceIssuedHistoryOperation(
            historyEntry.HistoryOperationId,
            historyEntry.AccountId,
            historyEntry.AccountNumber,
            new InvoiceId(issuedPayload.InvoiceId),
            historyEntry.CreatedTime);

        return new DeserializationHistoryOperationResult.Success(invoiceIssued);
    }
}