using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using SourceKit.Generators.Builder.Annotations;

namespace BankSystemApi.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record AccountQuery(
    AccountNumber[] AccountNumbers,
    AccountId[] AccountIds,
    UserId[] UserIds,
    AccountId? AccountIdCursor,
    [RequiredValue] int PageSize);