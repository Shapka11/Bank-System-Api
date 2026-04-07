using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;
using AccountQuery = Bsa.Application.Abstractions.Persistence.Queries.AccountQuery;

namespace Bsa.Application.Abstractions.Persistence.Repositories;

public interface IAccountRepository
{
    IAsyncEnumerable<Account> AddAsync(
        IReadOnlyCollection<Account> accounts,
        CancellationToken cancellationToken);

    Task UpdateAsync(IReadOnlyCollection<Account> accounts, CancellationToken cancellationToken);

    IAsyncEnumerable<Account> QueryAsync(AccountQuery query, CancellationToken cancellationToken);

    Task<Account?> FindAccountByNumberAsync(AccountNumber number, CancellationToken cancellationToken);

    Task<Account?> FindAccountByIdAsync(AccountId accountId, CancellationToken cancellationToken);

    IAsyncEnumerable<Account> FindAccountByNumberAsync(
        IEnumerable<AccountNumber> accountNumbers,
        CancellationToken cancellationToken);
}