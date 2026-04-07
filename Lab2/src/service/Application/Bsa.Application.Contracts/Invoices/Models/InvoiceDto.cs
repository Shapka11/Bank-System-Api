namespace Bsa.Application.Contracts.Invoices.Models;

public record InvoiceDto(
    long Id,
    string SenderAccountNumber,
    string ReceiverAccountNumber,
    decimal Amount,
    InvoiceStatusDto Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);