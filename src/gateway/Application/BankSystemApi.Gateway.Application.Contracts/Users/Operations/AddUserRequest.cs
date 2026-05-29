namespace BankSystemApi.Gateway.Application.Contracts.Users.Operations;

public readonly record struct AddUserRequest(Guid AuthorizationId);