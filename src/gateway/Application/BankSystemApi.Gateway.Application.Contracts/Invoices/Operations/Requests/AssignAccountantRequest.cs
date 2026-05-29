namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct AssignAccountantRequest(Guid UserId, long InvoiceId, long AccountantId);