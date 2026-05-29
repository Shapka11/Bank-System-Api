using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Invoices.Results;
using BankSystemApi.Domain.Invoices.States;
using BankSystemApi.Domain.ValueObjects;

namespace BankSystemApi.Domain.Invoices;

public sealed class Invoice
{
    public Invoice(
        InvoiceId id,
        AccountId senderAccountId,
        AccountId receiverAccountId,
        Money amount,
        IInvoiceState state,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        SenderAccountId = senderAccountId;
        ReceiverAccountId = receiverAccountId;
        Amount = amount;
        State = state;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public InvoiceId Id { get; }

    public AccountId SenderAccountId { get; }

    public AccountId ReceiverAccountId { get; }

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

    public ApproveInvoiceResult Approve()
    {
        if (State.CanApprove() is false)
            return new ApproveInvoiceResult.Failure($"Cannot approve invoice in {State.State} state");

        State = new ApprovedInvoiceState();
        return new ApproveInvoiceResult.Success();
    }

    public DeclineInvoiceResult Decline()
    {
        if (State.CanDecline() is false)
            return new DeclineInvoiceResult.Failure($"Cannot decline invoice in {State.State} state");

        State = new DeclinedInvoiceState();
        return new DeclineInvoiceResult.Success();
    }
}