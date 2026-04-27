namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

public record InvoiceDto(
    long Id,
    Guid SenderAccountId,
    Guid ReceiverAccountId,
    decimal Amount,
    InvoiceStatusDto Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);