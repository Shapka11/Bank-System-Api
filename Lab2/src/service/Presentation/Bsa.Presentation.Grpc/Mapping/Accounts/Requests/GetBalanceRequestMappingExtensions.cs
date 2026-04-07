using Bsa.Application.Contracts.Accounts.Operations;

namespace Bsa.Presentation.Grpc.Mapping.Accounts.Requests;

public static class GetBalanceRequestMappingExtensions
{
    public static GetBalance.Request MapToApplication(this ProtoGetBalanceRequest protoRequest)
        => new(Guid.Parse(protoRequest.Id));
}