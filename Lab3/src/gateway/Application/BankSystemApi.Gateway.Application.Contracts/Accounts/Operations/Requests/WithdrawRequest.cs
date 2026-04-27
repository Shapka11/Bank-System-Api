namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;

public readonly record struct WithdrawRequest(Guid UserId, Guid AccountId, decimal Amount);