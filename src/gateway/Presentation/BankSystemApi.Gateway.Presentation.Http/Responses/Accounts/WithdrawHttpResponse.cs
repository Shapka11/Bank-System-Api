using BankSystemApi.Gateway.Presentation.Http.Models.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Responses.Accounts;

public readonly record struct WithdrawHttpResponse
{
    public required AccountModel Account { get; init; }
}