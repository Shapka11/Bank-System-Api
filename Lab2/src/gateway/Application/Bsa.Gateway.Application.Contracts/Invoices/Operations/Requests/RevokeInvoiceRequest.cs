namespace Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct RevokeInvoiceRequest(Guid SessionId, long InvoiceId);