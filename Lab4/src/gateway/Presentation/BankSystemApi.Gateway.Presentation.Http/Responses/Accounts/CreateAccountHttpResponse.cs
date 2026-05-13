using BankSystemApi.Gateway.Presentation.Http.Models.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Responses.Accounts;

public readonly record struct CreateAccountHttpResponse
{
    public required AccountModel Account { get; init; }
}