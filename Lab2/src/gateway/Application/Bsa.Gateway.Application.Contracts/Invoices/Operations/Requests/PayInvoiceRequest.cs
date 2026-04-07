namespace Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct PayInvoiceRequest(Guid SessionId, long InvoiceId);