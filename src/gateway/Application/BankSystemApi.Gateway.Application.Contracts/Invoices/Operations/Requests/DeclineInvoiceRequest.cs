namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct DeclineInvoiceRequest(Guid UserId, long InvoiceId);