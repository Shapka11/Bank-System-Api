using BankSystemApi.Domain.Users;
using SourceKit.Generators.Builder.Annotations;

namespace BankSystemApi.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record UserQuery(
    UserId[] Ids,
    Guid[] AuthorizationIds,
    UserId? SessionIdCursor,
    [RequiredValue] int PageSize);