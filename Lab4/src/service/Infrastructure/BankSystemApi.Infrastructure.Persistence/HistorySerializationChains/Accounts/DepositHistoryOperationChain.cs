using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.ValueObjects;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Accounts;

public sealed class DepositHistoryOperationChain : HistoryOperationSerializationChain
{
    public override SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
    {
        if (operation is not DepositHistoryOperation depositOperation)
            return NextChainElement.SerializeSingle(operation);

        var payload = new DepositPayload(depositOperation.Amount.Value);
        string payloadJson = JsonSerializer.Serialize<PayloadBase>(payload);

        return new SerializationHistoryOperationResult.Success([payloadJson]);
    }

    public override DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
    {
        if (historyEntry.Payload is not DepositPayload depositPayload)
            return NextChainElement.Deserialize(historyEntry);

        var deposit = new DepositHistoryOperation(
            historyEntry.HistoryOperationId,
            historyEntry.AccountId,
            new Money(depositPayload.Amount),
            historyEntry.CreatedAt);

        return new DeserializationHistoryOperationResult.Success(deposit);
    }
}