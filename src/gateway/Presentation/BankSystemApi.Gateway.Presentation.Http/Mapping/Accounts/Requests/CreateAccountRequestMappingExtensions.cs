using BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Accounts.Requests;

public static class CreateAccountRequestMappingExtensions
{
    public static CreateAccountRequest MapToApplication(
        this CreateAccountHttpRequest httpRequest,
        string callerUserId,
        long targetUserId)
        => new(
            Guid.Parse(callerUserId),
            targetUserId,
            httpRequest.AccountNumber,
            httpRequest.Password,
            httpRequest.AccountType.MapToDto());
}