using Bsa.Domain.Accounts;
using SourceKit.Generators.Builder.Annotations;

namespace Bsa.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record SessionQuery(
    Guid[] SessionIds,
    AccountId[] AccountIds,
    Guid? SessionIdCursor,
    [RequiredValue] int PageSize);