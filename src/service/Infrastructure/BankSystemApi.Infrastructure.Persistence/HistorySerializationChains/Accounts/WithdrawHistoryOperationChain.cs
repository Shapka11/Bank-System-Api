using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.ValueObjects;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Accounts;

public sealed class WithdrawHistoryOperationChain : HistoryOperationSerializationChain
{
    public override SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
    {
        if (operation is not WithdrawHistoryOperation withdrawOperation)
            return NextChainElement.SerializeSingle(operation);

        var payload = new WithdrawPayload(withdrawOperation.Amount.Value);
        string payloadJson = JsonSerializer.Serialize<PayloadBase>(payload);

        return new SerializationHistoryOperationResult.Success([payloadJson]);
    }

    public override DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
    {
        if (historyEntry.Payload is not WithdrawPayload withdrawPayload)
            return NextChainElement.Deserialize(historyEntry);

        var withdraw = new WithdrawHistoryOperation(
            historyEntry.HistoryOperationId,
            historyEntry.AccountId,
            new Money(withdrawPayload.Amount),
            historyEntry.CreatedAt);

        return new DeserializationHistoryOperationResult.Success(withdraw);
    }
}