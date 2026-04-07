using Bsa.Application.Contracts.Accounts.Operations;

namespace Bsa.Presentation.Grpc.Mapping.Accounts.Requests;

public static class CreateAccountRequestMappingExtensions
{
    public static CreateAccount.Request MapToApplication(this ProtoCreateAccountRequest protoRequest)
        => new(
            Guid.Parse(protoRequest.Id),
            protoRequest.AccountNumber,
            protoRequest.Password);
}