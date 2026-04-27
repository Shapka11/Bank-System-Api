namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct CreateInvoiceRequest(
    Guid UserId,
    Guid SenderAccountId,
    Guid ReceiverAccountId,
    decimal Amount);