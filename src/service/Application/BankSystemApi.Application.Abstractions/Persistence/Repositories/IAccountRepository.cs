using BankSystemApi.Domain.Accounts;
using AccountQuery = BankSystemApi.Application.Abstractions.Persistence.Queries.AccountQuery;

namespace BankSystemApi.Application.Abstractions.Persistence.Repositories;

public interface IAccountRepository
{
    IAsyncEnumerable<Account> AddAsync(IReadOnlyCollection<Account> accounts, CancellationToken cancellationToken);

    Task UpdateAsync(IReadOnlyCollection<Account> accounts, CancellationToken cancellationToken);

    IAsyncEnumerable<Account> QueryAsync(AccountQuery query, CancellationToken cancellationToken);
}