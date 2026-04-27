using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Infrastructure.Persistence.Models.Payloads;

namespace BankSystemApi.Infrastructure.Persistence.Models;

public sealed record HistoryOperationEntry(
    HistoryOperationId HistoryOperationId,
    AccountId AccountId,
    DateTimeOffset CreatedAt,
    PayloadBase? Payload);