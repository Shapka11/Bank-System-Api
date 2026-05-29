namespace BankSystemApi.Gateway.Application.Abstractions.Invoices.Models;

public sealed record BankInvoiceModel(
    long Id,
    long SenderAccountId,
    long ReceiverAccountId,
    decimal Amount,
    BankInvoiceStatusModel Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);