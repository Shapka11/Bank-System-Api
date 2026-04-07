namespace Bsa.Gateway.Application.Contracts.Accounts.Operations;

public readonly record struct DepositRequest(Guid Id, decimal Amount);