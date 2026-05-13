using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class GetOutgoingInvoicesRequestMappingExtensions
{
    public static GetOutgoingInvoicesRequest MapToApplication(
        this GetOutgoingInvoicesHttpRequest httpRequest,
        string userId)
        => new(
            Guid.Parse(userId),
            httpRequest.ReceiverAccountIds,
            httpRequest.Statuses.Select(s => s.MapToDto()),
            httpRequest.PageSize,
            httpRequest.PageToken);
}