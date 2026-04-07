using Bsa.Gateway.Application.Contracts.Accounts.Operations;
using Bsa.Gateway.Presentation.Http.Requests.Accounts;

namespace Bsa.Gateway.Presentation.Http.Mapping.Accounts.Requests;

public static class DepositRequestMappingExtensions
{
    public static DepositRequest MapToApplication(this DepositHttpRequest httpRequest)
        => new(httpRequest.SessionId, httpRequest.Amount);
}