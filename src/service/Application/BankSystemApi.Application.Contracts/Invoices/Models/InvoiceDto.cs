namespace BankSystemApi.Application.Contracts.Invoices.Models;

public record InvoiceDto(
    long Id,
    long SenderAccountId,
    long ReceiverAccountId,
    decimal Amount,
    InvoiceStatusDto Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);