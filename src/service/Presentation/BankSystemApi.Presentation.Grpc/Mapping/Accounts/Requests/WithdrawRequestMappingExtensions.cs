using BankSystemApi.Application.Contracts.Accounts.Operations;

namespace BankSystemApi.Presentation.Grpc.Mapping.Accounts.Requests;

public static class WithdrawRequestMappingExtensions
{
    public static Withdraw.Request MapToApplication(this ProtoWithdrawRequest protoRequest)
        => new(
            Guid.Parse(protoRequest.UserId),
            protoRequest.AccountId,
            protoRequest.Amount.DecimalValue);
}