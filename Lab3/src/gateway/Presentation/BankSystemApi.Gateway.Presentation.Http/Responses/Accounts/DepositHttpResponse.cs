using BankSystemApi.Gateway.Presentation.Http.Models.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Responses.Accounts;

public readonly record struct DepositHttpResponse
{
    public required AccountModel Account { get; init; }
}