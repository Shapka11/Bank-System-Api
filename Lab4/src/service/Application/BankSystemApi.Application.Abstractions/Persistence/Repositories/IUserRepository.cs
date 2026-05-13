using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Domain.Users;

namespace BankSystemApi.Application.Abstractions.Persistence.Repositories;

public interface IUserRepository
{
    Task<AddUserResult> TryAddAsync(IReadOnlyCollection<User> users, CancellationToken cancellationToken);

    IAsyncEnumerable<User> QueryAsync(UserQuery query, CancellationToken cancellationToken);
}