namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;

public readonly record struct DepositRequest(Guid UserId, long AccountId, decimal Amount);