using BankSystemApi.Application.Contracts.Accounts.Operations;

namespace BankSystemApi.Presentation.Grpc.Mapping.Accounts.Requests;

public static class CreateAccountRequestMappingExtensions
{
    public static CreateAccount.Request MapToApplication(this ProtoCreateAccountRequest protoRequest)
        => new(
            Guid.Parse(protoRequest.CallerUserId),
            protoRequest.TargetUserId,
            protoRequest.AccountNumber,
            protoRequest.Password);
}