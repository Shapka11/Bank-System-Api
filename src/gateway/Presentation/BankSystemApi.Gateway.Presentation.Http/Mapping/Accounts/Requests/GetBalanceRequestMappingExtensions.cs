using BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Accounts.Requests;

public static class GetBalanceRequestMappingExtensions
{
    public static GetBalanceRequest MapToApplication(this GetBalanceHttpRequest httpRequest, string userId)
        => new(Guid.Parse(userId), httpRequest.AccountId);
}