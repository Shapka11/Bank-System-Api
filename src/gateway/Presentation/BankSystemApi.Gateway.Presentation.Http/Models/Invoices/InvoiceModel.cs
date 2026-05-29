namespace BankSystemApi.Gateway.Presentation.Http.Models.Invoices;

public sealed record InvoiceModel(
    long Id,
    long SenderAccountId,
    long ReceiverAccountId,
    decimal Amount,
    InvoiceStatusModel Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);