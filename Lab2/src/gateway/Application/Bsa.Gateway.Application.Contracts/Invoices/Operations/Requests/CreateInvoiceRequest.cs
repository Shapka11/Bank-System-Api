namespace Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct CreateInvoiceRequest(
    Guid SessionId,
    string SenderAccountNumber,
    string ReceiverAccountNumber,
    decimal Amount);