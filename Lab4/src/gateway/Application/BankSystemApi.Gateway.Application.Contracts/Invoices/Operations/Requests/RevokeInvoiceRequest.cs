namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct RevokeInvoiceRequest(Guid UserId, long InvoiceId);