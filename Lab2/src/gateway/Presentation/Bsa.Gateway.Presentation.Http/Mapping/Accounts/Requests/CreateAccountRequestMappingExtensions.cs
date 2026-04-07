using Bsa.Gateway.Application.Contracts.Accounts.Operations;
using Bsa.Gateway.Presentation.Http.Requests.Accounts;

namespace Bsa.Gateway.Presentation.Http.Mapping.Accounts.Requests;

public static class CreateAccountRequestMappingExtensions
{
    public static CreateAccountRequest MapToApplication(this CreateAccountHttpRequest httpRequest)
        => new(httpRequest.SessionId, httpRequest.AccountNumber, httpRequest.Password);
}