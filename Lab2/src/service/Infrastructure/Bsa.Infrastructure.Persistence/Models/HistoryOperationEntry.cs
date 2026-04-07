using Bsa.Domain.Accounts;
using Bsa.Domain.HistoryOperations;
using Bsa.Domain.ValueObjects;
using Bsa.Infrastructure.Persistence.Models.Payloads;

namespace Bsa.Infrastructure.Persistence.Models;

public sealed record HistoryOperationEntry(
    HistoryOperationId HistoryOperationId,
    AccountId AccountId,
    AccountNumber AccountNumber,
    DateTimeOffset CreatedTime,
    PayloadBase? Payload);