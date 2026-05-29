namespace BankSystemApi.Application.Abstractions.Events.Models;

public sealed record CreationInvoiceEvent(
    long InvoiceId,
    long SenderAccountId,
    long ReceiverAccountId,
    decimal Amount);