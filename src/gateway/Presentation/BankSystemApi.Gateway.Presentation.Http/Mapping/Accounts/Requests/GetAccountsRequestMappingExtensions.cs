using BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Accounts.Requests;

public static class GetAccountsRequestMappingExtensions
{
    public static GetAccountsRequest MapToApplication(this GetAccountsHttpRequest request, string userId) =>
        new(Guid.Parse(userId), request.PageSize, request.PageToken);
}