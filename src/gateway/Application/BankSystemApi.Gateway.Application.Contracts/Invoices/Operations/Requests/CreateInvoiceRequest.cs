namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct CreateInvoiceRequest(
    Guid UserId,
    long SenderAccountId,
    long ReceiverAccountId,
    decimal Amount);