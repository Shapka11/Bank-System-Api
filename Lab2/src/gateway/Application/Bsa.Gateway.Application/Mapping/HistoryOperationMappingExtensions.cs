using Bsa.Gateway.Application.Abstractions.HistoryOperations.Models;
using Bsa.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;
using Bsa.Gateway.Application.Abstractions.HistoryOperations.Models.Invoices;
using Bsa.Gateway.Application.Contracts.HistoryOperations.Models;
using Bsa.Gateway.Application.Contracts.HistoryOperations.Models.Accounts;
using Bsa.Gateway.Application.Contracts.HistoryOperations.Models.Invoices;

namespace Bsa.Gateway.Application.Mapping;

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
            _ => throw new InvalidOperationException(),
        };
    }

    public static IEnumerable<HistoryOperationDto> MapToDto(this IEnumerable<BankHistoryOperationModel> entities)
        => entities.Select(MapToDto);

    public static CreateAccountHistoryOperationDto MapToDto(this CreateAccountBankHistoryOperationModel model)
        => new CreateAccountHistoryOperationDto(
            model.Id,
            model.AccountId,
            model.AccountNumber,
            model.OccurredAt);

    public static CheckBalanceHistoryOperationDto MapToDto(this CheckBalanceBankHistoryOperationModel model)
        => new CheckBalanceHistoryOperationDto(
            model.Id,
            model.AccountId,
            model.AccountNumber,
            model.Balance,
            model.OccurredAt);

    public static DepositHistoryOperationDto MapToDto(this DepositBankHistoryOperationModel model)
        => new DepositHistoryOperationDto(
            model.Id,
            model.AccountId,
            model.AccountNumber,
            model.Amount,
            model.OccurredAt);

    public static WithdrawHistoryOperationDto MapToDto(this WithdrawBankHistoryOperationModel model)
        => new WithdrawHistoryOperationDto(
            model.Id,
            model.AccountId,
            model.AccountNumber,
            model.Amount,
            model.OccurredAt);

    public static InvoiceIssuedHistoryOperationDto MapToDto(this InvoiceIssuedBankHistoryOperationModel model)
        => new InvoiceIssuedHistoryOperationDto(
            model.Id,
            model.AccountId,
            model.AccountNumber,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoicePaymentReceivedHistoryOperationDto MapToDto(
        this InvoicePaymentReceivedBankHistoryOperationModel model)
        => new InvoicePaymentReceivedHistoryOperationDto(
            model.Id,
            model.AccountId,
            model.AccountNumber,
            model.Amount,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoicePaymentSentHistoryOperationDto MapToDto(this InvoicePaymentSentBankHistoryOperationModel model)
        => new InvoicePaymentSentHistoryOperationDto(
            model.Id,
            model.AccountId,
            model.AccountNumber,
            model.Amount,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoiceReceivedHistoryOperationDto MapToDto(this InvoiceReceivedBankHistoryOperationModel model)
        => new InvoiceReceivedHistoryOperationDto(
            model.Id,
            model.AccountId,
            model.AccountNumber,
            model.InvoiceId,
            model.OccurredAt);

    public static InvoiceRevokedHistoryOperationDto MapToDto(this InvoiceRevokedBankHistoryOperationModel model)
        => new InvoiceRevokedHistoryOperationDto(
            model.Id,
            model.AccountId,
            model.AccountNumber,
            model.InvoiceId,
            model.OccurredAt);
}