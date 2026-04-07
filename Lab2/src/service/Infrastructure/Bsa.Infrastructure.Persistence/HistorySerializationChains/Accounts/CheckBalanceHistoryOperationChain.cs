using Bsa.Domain.HistoryOperations;
using Bsa.Domain.HistoryOperations.Accounts;
using Bsa.Domain.ValueObjects;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;
using Bsa.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Accounts;

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
            historyEntry.AccountNumber,
            new Money(checkBalancePayload.Balance),
            historyEntry.CreatedTime);

        return new DeserializationHistoryOperationResult.Success(checkBalance);
    }
}