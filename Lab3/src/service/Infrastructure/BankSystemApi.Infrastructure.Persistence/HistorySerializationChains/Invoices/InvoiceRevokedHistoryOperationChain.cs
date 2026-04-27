using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Invoices;

public sealed class InvoiceRevokedHistoryOperationChain : HistoryOperationSerializationChain
{
    public override SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
    {
        if (operation is not InvoiceRevokedHistoryOperation invoiceRevokedOperation)
            return NextChainElement.SerializeSingle(operation);

        var payload = new InvoiceRevokedPayload(invoiceRevokedOperation.InvoiceId.Value);
        string payloadJson = JsonSerializer.Serialize<PayloadBase>(payload);

        return new SerializationHistoryOperationResult.Success([payloadJson]);
    }

    public override DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
    {
        if (historyEntry.Payload is not InvoiceRevokedPayload revokedPayload)
            return NextChainElement.Deserialize(historyEntry);

        var deposit = new InvoiceRevokedHistoryOperation(
            historyEntry.HistoryOperationId,
            historyEntry.AccountId,
            new InvoiceId(revokedPayload.InvoiceId),
            historyEntry.CreatedAt);

        return new DeserializationHistoryOperationResult.Success(deposit);
    }
}