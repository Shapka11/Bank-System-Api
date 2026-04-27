namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;

public readonly record struct GetAccountsRequest(Guid UserId, int PageSize, string? PageToken);