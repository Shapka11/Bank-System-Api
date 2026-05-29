using BankSystemApi.Gateway.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;

public readonly record struct CreateAccountRequest(
    Guid CallerUserId,
    long TargetUserId,
    string AccountNumber,
    string Password,
    AccountTypeDto AccountType);