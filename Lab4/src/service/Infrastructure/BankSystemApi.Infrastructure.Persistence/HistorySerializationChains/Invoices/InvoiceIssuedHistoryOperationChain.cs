using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Invoices;

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
            new InvoiceId(issuedPayload.InvoiceId),
            historyEntry.CreatedAt);

        return new DeserializationHistoryOperationResult.Success(invoiceIssued);
    }
}