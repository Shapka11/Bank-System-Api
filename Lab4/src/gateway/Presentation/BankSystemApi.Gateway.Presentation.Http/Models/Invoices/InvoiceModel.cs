namespace BankSystemApi.Gateway.Presentation.Http.Models.Invoices;

public sealed record InvoiceModel(
    long Id,
    Guid SenderAccountId,
    Guid ReceiverAccountId,
    decimal Amount,
    InvoiceStatusModel Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);