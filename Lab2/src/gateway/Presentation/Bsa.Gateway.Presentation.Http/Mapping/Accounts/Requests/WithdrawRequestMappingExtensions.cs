using Bsa.Gateway.Application.Contracts.Accounts.Operations;
using Bsa.Gateway.Presentation.Http.Requests.Accounts;

namespace Bsa.Gateway.Presentation.Http.Mapping.Accounts.Requests;

public static class WithdrawRequestMappingExtensions
{
    public static WithdrawRequest MapToApplication(this WithdrawHttpRequest httpRequest)
        => new(httpRequest.SessionId, httpRequest.Amount);
}