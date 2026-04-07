using Bsa.Application.Abstractions.Persistence.Queries;

namespace Bsa.Infrastructure.Persistence.Specifications;

public static class SessionSpecifications
{
    public static SessionQuery ById(Guid id)
    {
        return SessionQuery.Build(builder => builder
            .WithSessionId(id)
            .WithPageSize(1));
    }
}