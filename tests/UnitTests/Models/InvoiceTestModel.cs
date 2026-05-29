using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.Invoices.States;
using BankSystemApi.Domain.ValueObjects;

namespace UnitTests.Models;

public sealed record InvoiceTestModel(
    InvoiceId Id,
    AccountId SenderAccountId,
    AccountId ReceiverAccountId,
    Money Amount,
    IInvoiceState State,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public Invoice MapToDomain()
        => new(
            Id,
            SenderAccountId,
            ReceiverAccountId,
            Amount,
            State,
            CreatedAt,
            UpdatedAt);
}