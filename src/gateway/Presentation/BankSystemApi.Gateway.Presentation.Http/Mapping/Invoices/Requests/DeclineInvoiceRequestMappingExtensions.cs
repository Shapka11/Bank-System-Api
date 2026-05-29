using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class DeclineInvoiceRequestMappingExtensions
{
    public static DeclineInvoiceRequest MapToApplication(this DeclineInvoiceHttpRequest httpRequest, string userId)
        => new(Guid.Parse(userId), httpRequest.InvoiceId);
}