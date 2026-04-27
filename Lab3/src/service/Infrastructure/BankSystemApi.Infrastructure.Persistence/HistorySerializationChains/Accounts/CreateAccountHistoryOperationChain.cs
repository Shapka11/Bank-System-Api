using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;
using System.Text.Json;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Accounts;

public sealed class CreateAccountHistoryOperationChain : HistoryOperationSerializationChain
{
    public override SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
    {
        if (operation is not CreateAccountHistoryOperation)
            return NextChainElement.SerializeSingle(operation);

        var payload = new CreateAccountPayload();
        string payloadJson = JsonSerializer.Serialize<PayloadBase>(payload);

        return new SerializationHistoryOperationResult.Success([payloadJson]);
    }

    public override DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
    {
        if (historyEntry.Payload is not CreateAccountPayload)
            return NextChainElement.Deserialize(historyEntry);

        var createAccount = new CreateAccountHistoryOperation(
            historyEntry.HistoryOperationId,
            historyEntry.AccountId,
            historyEntry.CreatedAt);

        return new DeserializationHistoryOperationResult.Success(createAccount);
    }
}