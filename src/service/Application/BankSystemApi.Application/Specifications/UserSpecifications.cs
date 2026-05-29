using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Domain.Users;
using UserQuery = BankSystemApi.Application.Abstractions.Persistence.Queries.UserQuery;

namespace BankSystemApi.Application.Specifications;

public static class UserSpecifications
{
    public static ValueTask<User?> FindByAuthorizationIdAsync(
        this IUserRepository repository,
        Guid authorizationId,
        CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = UserQuery.Build(builder => builder
            .WithAuthorizationId(authorizationId)
            .WithPageSize(pageSize));

        return repository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }

    public static ValueTask<User?> FindByIdAsync(
        this IUserRepository repository,
        UserId id,
        CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = UserQuery.Build(builder => builder
            .WithId(id)
            .WithPageSize(pageSize));

        return repository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}