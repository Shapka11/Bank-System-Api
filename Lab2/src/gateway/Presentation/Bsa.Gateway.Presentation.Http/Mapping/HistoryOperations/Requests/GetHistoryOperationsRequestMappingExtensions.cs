using Bsa.Gateway.Application.Contracts.HistoryOperations.Operations;
using Bsa.Gateway.Presentation.Http.Requests.HistoryOperations;

namespace Bsa.Gateway.Presentation.Http.Mapping.HistoryOperations.Requests;

public static class GetHistoryOperationsRequestMappingExtensions
{
    public static GetHistoryOperationsRequest MapToApplication(this GetHistoryHttpRequest httpRequest)
        => new GetHistoryOperationsRequest(httpRequest.SessionId, httpRequest.PageSize, httpRequest.PageToken);
}