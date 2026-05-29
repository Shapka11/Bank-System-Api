using BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Accounts.Requests;

public static class DepositRequestMappingExtensions
{
    public static DepositRequest MapToApplication(this DepositHttpRequest httpRequest, string userId)
        => new(Guid.Parse(userId), httpRequest.AccountId, httpRequest.Amount);
}