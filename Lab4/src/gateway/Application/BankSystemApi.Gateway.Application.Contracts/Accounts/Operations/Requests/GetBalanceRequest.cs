namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;

public readonly record struct GetBalanceRequest(Guid UserId, Guid AccountId);