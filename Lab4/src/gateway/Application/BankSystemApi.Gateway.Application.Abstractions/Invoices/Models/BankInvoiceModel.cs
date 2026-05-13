namespace BankSystemApi.Gateway.Application.Abstractions.Invoices.Models;

public sealed record BankInvoiceModel(
    long Id,
    Guid SenderAccountId,
    Guid ReceiverAccountId,
    decimal Amount,
    BankInvoiceStatusModel Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);