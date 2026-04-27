using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Invoices;

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
            new InvoiceId(receivedPayload.InvoiceId),
            historyEntry.CreatedAt);

        return new DeserializationHistoryOperationResult.Success(deposit);
    }
}