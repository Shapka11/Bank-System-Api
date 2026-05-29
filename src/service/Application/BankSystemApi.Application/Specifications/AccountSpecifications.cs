using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using AccountQuery = BankSystemApi.Application.Abstractions.Persistence.Queries.AccountQuery;

namespace BankSystemApi.Application.Specifications;

public static class AccountSpecifications
{
    public static ValueTask<Account?> FindAccountByNumberAsync(
        this IAccountRepository repository,
        AccountNumber number,
        CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = AccountQuery.Build(builder => builder
            .WithAccountNumber(number)
            .WithPageSize(pageSize));

        return repository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }

    public static ValueTask<Account?> FindAccountByIdAsync(
        this IAccountRepository repository,
        AccountId accountId,
        CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = AccountQuery.Build(builder => builder
            .WithAccountId(accountId)
            .WithPageSize(pageSize));

        return repository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }

    public static ValueTask<int> GetTotalByUserId(
        this IAccountRepository repository,
        UserId userId,
        CancellationToken cancellationToken)
    {
        var query = AccountQuery.Build(builder => builder
            .WithUserId(userId)
            .WithPageSize(int.MaxValue));

        return repository.QueryAsync(query, cancellationToken).CountAsync(cancellationToken);
    }

    public static ValueTask<Account[]> GetAllByUserId(
        this IAccountRepository repository,
        UserId userId,
        CancellationToken cancellationToken)
    {
        var query = AccountQuery.Build(builder => builder
            .WithUserId(userId)
            .WithPageSize(int.MaxValue));

        return repository.QueryAsync(query, cancellationToken).ToArrayAsync(cancellationToken);
    }
}