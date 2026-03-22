using Bsa.Domain.Sessions;
using SourceKit.Generators.Builder.Annotations;

namespace Bsa.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record AdminSessionQuery(
    AdminSession[] Sessions,
    Guid? SessionIdCursor,
    [RequiredValue] int PageSize);