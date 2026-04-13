using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;

namespace Bsa.Application.Specifications;

public sealed class AccountSpecifications
{
    private readonly IAccountRepository _accountRepository;

    public AccountSpecifications(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account?> FindAccountByNumberAsync(AccountNumber number, CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = AccountQuery.Build(builder => builder
            .WithAccountNumber(number)
            .WithPageSize(pageSize));

        return await _accountRepository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<Account?> FindAccountByIdAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = AccountQuery.Build(builder => builder
            .WithAccountId(accountId)
            .WithPageSize(pageSize));

        return await _accountRepository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}