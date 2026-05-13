namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct PayInvoiceRequest(Guid UserId, long InvoiceId);