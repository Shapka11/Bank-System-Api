using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models;
using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models.Accounts;
using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models.Invoices;
using BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations;
using BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;
using BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.HistoryOperations;

public static class HistoryOperationMappingExtensions
{
    public static HistoryOperationModel MapToModel(this HistoryOperationDto dto)
    {
        return dto switch
        {
            CheckBalanceHistoryOperationDto checkBalance => checkBalance.MapToModel(),
            CreateAccountHistoryOperationDto createAccount => createAccount.MapToModel(),
            DepositHistoryOperationDto deposit => deposit.MapToModel(),
            WithdrawHistoryOperationDto withdraw => withdraw.MapToModel(),
            InvoiceIssuedHistoryOperationDto invoiceIssued => invoiceIssued.MapToModel(),
            InvoicePaymentReceivedHistoryOperationDto invoicePaymentReceived => invoicePaymentReceived.MapToModel(),
            InvoicePaymentSentHistoryOperationDto invoicePaymentSent => invoicePaymentSent.MapToModel(),
            InvoiceReceivedHistoryOperationDto invoiceReceived => invoiceReceived.MapToModel(),
            InvoiceRevokedHistoryOperationDto invoiceRevoked => invoiceRevoked.MapToModel(),
            _ => throw new InvalidOperationException($"Unknown DTO type: {dto.GetType().Name}"),
        };
    }

    public static CreateAccountHistoryOperationModel MapToModel(this CreateAccountHistoryOperationDto dto)
        => new(
            dto.Id,
            dto.AccountId,
            dto.OccurredAt);

    public static CheckBalanceHistoryOperationModel MapToModel(this CheckBalanceHistoryOperationDto dto)
        => new(
            dto.Id,
            dto.AccountId,
            dto.Balance,
            dto.OccurredAt);

    public static DepositHistoryOperationModel MapToModel(this DepositHistoryOperationDto dto)
        => new(
            dto.Id,
            dto.AccountId,
            dto.Amount,
            dto.OccurredAt);

    public static WithdrawHistoryOperationModel MapToModel(this WithdrawHistoryOperationDto dto)
        => new(
            dto.Id,
            dto.AccountId,
            dto.Amount,
            dto.OccurredAt);

    public static InvoiceIssuedHistoryOperationModel MapToModel(this InvoiceIssuedHistoryOperationDto dto)
        => new(
            dto.Id,
            dto.AccountId,
            dto.InvoiceId,
            dto.OccurredAt);

    public static InvoicePaymentReceivedHistoryOperationModel MapToModel(
        this InvoicePaymentReceivedHistoryOperationDto dto)
        => new(
            dto.Id,
            dto.AccountId,
            dto.Amount,
            dto.InvoiceId,
            dto.OccurredAt);

    public static InvoicePaymentSentHistoryOperationModel MapToModel(this InvoicePaymentSentHistoryOperationDto dto)
        => new(
            dto.Id,
            dto.AccountId,
            dto.Amount,
            dto.InvoiceId,
            dto.OccurredAt);

    public static InvoiceReceivedHistoryOperationModel MapToModel(this InvoiceReceivedHistoryOperationDto dto)
        => new(
            dto.Id,
            dto.AccountId,
            dto.InvoiceId,
            dto.OccurredAt);

    public static InvoiceRevokedHistoryOperationModel MapToModel(this InvoiceRevokedHistoryOperationDto dto)
        => new(
            dto.Id,
            dto.AccountId,
            dto.InvoiceId,
            dto.OccurredAt);
}