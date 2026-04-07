namespace Bsa.Gateway.Application.Contracts.Accounts.Operations;

public readonly record struct CreateAccountRequest(Guid Id, string AccountNumber, string Password);