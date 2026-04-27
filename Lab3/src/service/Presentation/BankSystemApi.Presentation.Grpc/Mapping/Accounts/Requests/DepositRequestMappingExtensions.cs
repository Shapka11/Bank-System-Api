using BankSystemApi.Application.Contracts.Accounts.Operations;

namespace BankSystemApi.Presentation.Grpc.Mapping.Accounts.Requests;

public static class DepositRequestMappingExtensions
{
    public static Deposit.Request MapToApplication(this ProtoDepositRequest protoRequest)
        => new(
            Guid.Parse(protoRequest.UserId),
            Guid.Parse(protoRequest.AccountId),
            protoRequest.Amount.DecimalValue);
}