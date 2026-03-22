using SourceKit.Generators.Builder.Annotations;

namespace Bsa.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record UserSessionQuery(
    Guid[] SessionIds,
    Guid? SessionIdCursor,
    [RequiredValue] int PageSize);