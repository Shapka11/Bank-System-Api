using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Domain.Users;

namespace BankSystemApi.Application.Abstractions.Persistence.Repositories;

public interface IUserRepository
{
    Task<bool> TryAddAsync(IReadOnlyCollection<User> users, CancellationToken cancellationToken);

    IAsyncEnumerable<User> QueryAsync(UserQuery query, CancellationToken cancellationToken);
}