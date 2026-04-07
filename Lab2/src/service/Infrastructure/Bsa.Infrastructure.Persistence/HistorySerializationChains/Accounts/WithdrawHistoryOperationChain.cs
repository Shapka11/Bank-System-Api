using Bsa.Domain.HistoryOperations;
using Bsa.Domain.HistoryOperations.Accounts;
using Bsa.Domain.ValueObjects;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;
using Bsa.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Accounts;

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
            historyEntry.AccountNumber,
            new Money(withdrawPayload.Amount),
            historyEntry.CreatedTime);

        return new DeserializationHistoryOperationResult.Success(withdraw);
    }
}