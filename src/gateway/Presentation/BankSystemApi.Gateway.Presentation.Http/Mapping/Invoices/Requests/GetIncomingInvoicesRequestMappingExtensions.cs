using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class GetIncomingInvoicesRequestMappingExtensions
{
    public static GetIncomingInvoicesRequest MapToApplication(
        this GetIncomingInvoicesHttpRequest httpRequest,
        string userId)
        => new(
            Guid.Parse(userId),
            httpRequest.SenderAccountIds,
            httpRequest.Statuses.Select(s => s.MapToDto()),
            httpRequest.PageSize,
            httpRequest.PageToken);
}