using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;
using SourceKit.Generators.Builder.Annotations;

namespace Bsa.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record AccountQuery(
    AccountNumber[] AccountNumbers,
    AccountId[] AccountIds,
    AccountId? AccountIdCursor,
    [RequiredValue] int PageSize);