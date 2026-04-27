using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using SourceKit.Generators.Builder.Annotations;

namespace BankSystemApi.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record OperationHistoryQuery(
    AccountId[] AccountIds,
    HistoryOperationId? IdCursor,
    [RequiredValue] int PageSize);