using BankSystemApi.Application.Contracts.Accounts.Operations;

namespace BankSystemApi.Presentation.Grpc.Mapping.Accounts.Requests;

public static class GetBalanceRequestMappingExtensions
{
    public static GetBalance.Request MapToApplication(this ProtoGetBalanceRequest protoRequest)
        => new(Guid.Parse(protoRequest.UserId), Guid.Parse(protoRequest.AccountId));
}