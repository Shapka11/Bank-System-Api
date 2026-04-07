namespace Bsa.Gateway.Presentation.Http.Models.Invoices;

public sealed record InvoiceModel(
    long Id,
    string SenderAccountNumber,
    string ReceiverAccountNumber,
    decimal Amount,
    InvoiceStatusModel Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);