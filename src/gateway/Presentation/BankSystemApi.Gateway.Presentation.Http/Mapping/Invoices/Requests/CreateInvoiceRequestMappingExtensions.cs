using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class CreateInvoiceRequestMappingExtensions
{
    public static CreateInvoiceRequest MapToApplication(this CreateInvoiceHttpRequest httpRequest, string userId)
        => new(
            Guid.Parse(userId),
            httpRequest.SenderAccountId,
            httpRequest.ReceiverAccountId,
            httpRequest.Amount);
}