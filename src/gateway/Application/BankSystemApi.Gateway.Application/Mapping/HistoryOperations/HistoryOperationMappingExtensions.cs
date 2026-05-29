using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models;
using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;
using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Invoices;
using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models;
using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models.Accounts;
using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models.Invoices;

namespace BankSystemApi.Gateway.Application.Mapping.HistoryOperations;

public static class HistoryOperationMappingExtensions
{
    public static HistoryOperationDto MapToDto(this BankHistoryOperationModel model)
    {
        return model switch
        {
            CheckBalanceBankHistoryOperationModel checkBalance => checkBalance.MapToDto(),
            CreateAccountBankHistoryOperationModel createAccount => createAccount.MapToDto(),
            DepositBankHistoryOperationModel deposit => deposit.MapToDto(),
            WithdrawBankHistoryOperationModel withdraw => withdraw.MapToDto(),
            InvoiceIssuedBankHistoryOperationModel invoiceIssued => invoiceIssued.MapToDto(),
            InvoicePaymentReceivedBankHistoryOperationModel invoicePaymentReceived => invoicePaymentReceived.MapToDto(),
            InvoicePaymentSentBankHistoryOperationModel invoicePaymentSent => invoicePaymentSent.MapToDto(),
            InvoiceReceivedBankHistoryOperationModel invoiceReceived => invoiceReceived.MapToDto(),
            InvoiceRevokedBankHistoryOperationModel invoiceRevoked => invoiceRevoked.MapToDto(),
            InvoiceApprovedBankHistoryOperationModel invoiceApproved => invoiceApproved.MapToDto(),
            InvoiceDeclinedBankHistoryOperationModel invoiceDeclined => invoiceDeclined.MapToDto(),
            _ => throw new InvalidOperationException(),
        };
    }

    public static CreateAccountHistoryOperationDto MapToDto(this CreateAccountBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.OccurredAt);

    public static CheckBalanceHistoryOperationDto MapToDto(this CheckBalanceBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.Balance,
            model.OccurredAt);

    public static DepositHistoryOperationDto MapToDto(this DepositBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.Amount,
            model.OccurredAt);

    public static WithdrawHistoryOperationDto MapToDto(this WithdrawBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.Amount,
            model.OccurredAt);

    public static InvoiceIssuedHistoryOperationDto MapToDto(this InvoiceIssuedBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoicePaymentReceivedHistoryOperationDto MapToDto(
        this InvoicePaymentReceivedBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.Amount,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoicePaymentSentHistoryOperationDto MapToDto(this InvoicePaymentSentBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.Amount,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoiceReceivedHistoryOperationDto MapToDto(this InvoiceReceivedBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoiceRevokedHistoryOperationDto MapToDto(this InvoiceRevokedBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoiceApprovedHistoryOperationDto MapToDto(this InvoiceApprovedBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoiceDeclinedHistoryOperationDto MapToDto(this InvoiceDeclinedBankHistoryOperationModel model)
        => new(
            model.Id,
            model.AccountId,
            model.InvoiceId,
            model.OccurredAt);
}