using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.ValueObjects;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Accounts;

public sealed class CheckBalanceHistoryOperationChain : HistoryOperationSerializationChain
{
    public override SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
    {
        if (operation is not CheckBalanceHistoryOperation checkBalanceOperation)
            return NextChainElement.SerializeSingle(operation);

        var payload = new CheckBalancePayload(checkBalanceOperation.Balance.Value);
        string payloadJson = JsonSerializer.Serialize<PayloadBase>(payload);

        return new SerializationHistoryOperationResult.Success([payloadJson]);
    }

    public override DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
    {
        if (historyEntry.Payload is not CheckBalancePayload checkBalancePayload)
            return NextChainElement.Deserialize(historyEntry);

        var checkBalance = new CheckBalanceHistoryOperation(
            historyEntry.HistoryOperationId,
            historyEntry.AccountId,
            new Money(checkBalancePayload.Balance),
            historyEntry.CreatedAt);

        return new DeserializationHistoryOperationResult.Success(checkBalance);
    }
}