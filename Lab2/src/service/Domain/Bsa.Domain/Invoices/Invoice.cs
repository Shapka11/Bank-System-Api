using Bsa.Domain.Invoices.Results;
using Bsa.Domain.Invoices.States;
using Bsa.Domain.ValueObjects;

namespace Bsa.Domain.Invoices;

public sealed class Invoice
{
    public Invoice(
        InvoiceId id,
        AccountNumber senderAccountNumber,
        AccountNumber receiverAccountNumber,
        Money amount,
        IInvoiceState state,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        SenderAccountNumber = senderAccountNumber;
        ReceiverAccountNumber = receiverAccountNumber;
        Amount = amount;
        State = state;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public InvoiceId Id { get; }

    public AccountNumber SenderAccountNumber { get; }

    public AccountNumber ReceiverAccountNumber { get; }

    public Money Amount { get; }

    public IInvoiceState State { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; }

    public PayInvoiceResult Pay()
    {
        if (State.CanPay() is false)
            return new PayInvoiceResult.Failure($"Cannot pay invoice in {State.State} state");

        State = new PaidInvoiceState();
        return new PayInvoiceResult.Success();
    }

    public RevokeInvoiceResult Revoke()
    {
        if (State.CanRevoke() is false)
            return new RevokeInvoiceResult.Failure($"Cannot revoke invoice in {State.State} state");

        State = new RevokedInvoiceState();
        return new RevokeInvoiceResult.Success();
    }
}