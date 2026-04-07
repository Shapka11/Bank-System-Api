using Bsa.Application.Contracts.Accounts.Operations;

namespace Bsa.Presentation.Grpc.Mapping.Accounts.Requests;

public static class DepositRequestMappingExtensions
{
    public static Deposit.Request MapToApplication(this ProtoDepositRequest protoRequest)
        => new(Guid.Parse(protoRequest.Id), protoRequest.Amount.DecimalValue);
}