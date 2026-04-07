using Bsa.Application.Contracts.Accounts.Operations;

namespace Bsa.Presentation.Grpc.Mapping.Accounts.Requests;

public static class WithdrawRequestMappingExtensions
{
    public static Withdraw.Request MapToApplication(this ProtoWithdrawRequest protoRequest)
        => new(Guid.Parse(protoRequest.Id), protoRequest.Amount.DecimalValue);
}