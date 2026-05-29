using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class ApproveInvoiceRequestMappingExtensions
{
    public static ApproveInvoiceRequest MapToApplication(this ApproveInvoiceHttpRequest httpRequest, string userId)
        => new(Guid.Parse(userId), httpRequest.InvoiceId);
}