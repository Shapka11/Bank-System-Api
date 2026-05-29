using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Operations;
using BankSystemApi.Gateway.Presentation.Http.Requests.HistoryOperations;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.HistoryOperations.Requests;

public static class GetHistoryOperationsRequestMappingExtensions
{
    public static GetHistoryOperationsRequest MapToApplication(this GetHistoryHttpRequest httpRequest, string userId)
        => new(
            Guid.Parse(userId),
            httpRequest.AccountId,
            httpRequest.PageSize,
            httpRequest.PageToken);
}