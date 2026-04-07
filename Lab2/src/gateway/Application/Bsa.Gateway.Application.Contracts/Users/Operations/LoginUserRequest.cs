namespace Bsa.Gateway.Application.Contracts.Users.Operations;

public readonly record struct LoginUserRequest(string AccountNumber, string Password);