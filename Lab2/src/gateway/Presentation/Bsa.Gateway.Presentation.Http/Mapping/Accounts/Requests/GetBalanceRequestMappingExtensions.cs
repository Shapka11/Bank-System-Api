using Bsa.Gateway.Application.Contracts.Accounts.Operations;
using Bsa.Gateway.Presentation.Http.Requests.Accounts;

namespace Bsa.Gateway.Presentation.Http.Mapping.Accounts.Requests;

public static class GetBalanceRequestMappingExtensions
{
    public static GetBalanceRequest MapToApplication(this GetBalanceHttpRequest httpRequest)
        => new(httpRequest.SessionId);
}