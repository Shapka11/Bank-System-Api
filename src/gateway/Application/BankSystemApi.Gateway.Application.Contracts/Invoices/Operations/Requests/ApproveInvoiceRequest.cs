namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct ApproveInvoiceRequest(Guid UserId, long InvoiceId);