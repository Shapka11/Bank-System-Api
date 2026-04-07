namespace Bsa.Gateway.Application.Contracts.Accounts.Operations;

public readonly record struct WithdrawRequest(Guid Id, decimal Amount);