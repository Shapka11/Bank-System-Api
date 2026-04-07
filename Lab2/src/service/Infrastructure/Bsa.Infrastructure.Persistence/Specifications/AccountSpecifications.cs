using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;

namespace Bsa.Infrastructure.Persistence.Specifications;

public static class AccountSpecifications
{
    public static AccountQuery ById(AccountId accountId)
    {
        return AccountQuery.Build(builder => builder
            .WithAccountId(accountId)
            .WithPageSize(1));
    }

    public static AccountQuery ByNumbers(IEnumerable<AccountNumber> accountNumbers)
    {
        AccountNumber[] accountNumbersArray = accountNumbers.ToArray();

        return AccountQuery.Build(builder => builder
            .WithAccountNumbers(accountNumbersArray)
            .WithPageSize(accountNumbersArray.Length));
    }
}