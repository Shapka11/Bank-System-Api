using Bsa.Domain.Accounts;
using SourceKit.Generators.Builder.Annotations;

namespace Bsa.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record AccountOperationQuery(
    AccountId[] AccountIds,
    AccountId? IdCursor,
    [RequiredValue] int PageSize);