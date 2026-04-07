namespace Bsa.Gateway.Application.Abstractions.Invoices.Models;

public sealed record BankInvoiceModel(
    long Id,
    string SenderAccountNumber,
    string ReceiverAccountNumber,
    decimal Amount,
    BankInvoiceStatusModel Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);