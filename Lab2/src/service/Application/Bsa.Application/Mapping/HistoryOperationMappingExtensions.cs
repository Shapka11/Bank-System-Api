using Bsa.Application.Contracts.HistoryOperations.Models;
using Bsa.Application.Contracts.HistoryOperations.Models.Accounts;
using Bsa.Application.Contracts.HistoryOperations.Models.Invoices;
using Bsa.Domain.HistoryOperations;
using Bsa.Domain.HistoryOperations.Accounts;
using Bsa.Domain.HistoryOperations.Invoices;

namespace Bsa.Application.Mapping;

public static class HistoryOperationMappingExtensions
{
    public static HistoryOperationDto MapToDto(this HistoryOperation operation)
    {
        return operation switch
        {
            CreateAccountHistoryOperation op => op.MapToDto(),
            CheckBalanceHistoryOperation op => op.MapToDto(),
            DepositHistoryOperation op => op.MapToDto(),
            WithdrawHistoryOperation op => op.MapToDto(),
            InvoiceIssuedHistoryOperation op => op.MapToDto(),
            InvoicePaymentReceivedHistoryOperation op => op.MapToDto(),
            InvoicePaymentSentHistoryOperation op => op.MapToDto(),
            InvoiceReceivedHistoryOperation op => op.MapToDto(),
            InvoiceRevokedHistoryOperation op => op.MapToDto(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(operation),
                $"Mapping not supported for {operation.GetType().Name}"),
        };
    }

    public static IEnumerable<HistoryOperationDto> MapToDto(this HistoryOperation[] operations)
        => operations.Select(operation => operation.MapToDto()).ToArray();

    public static CreateAccountHistoryOperationDto MapToDto(this CreateAccountHistoryOperation operation)
        => new CreateAccountHistoryOperationDto(
            operation.Id.Value,
            operation.AccountId.Value,
            operation.AccountNumber.Value,
            operation.OccurredAt);

    public static CheckBalanceHistoryOperationDto MapToDto(this CheckBalanceHistoryOperation operation)
        => new CheckBalanceHistoryOperationDto(
            operation.Id.Value,
            operation.AccountId.Value,
            operation.AccountNumber.Value,
            operation.Balance.Value,
            operation.OccurredAt);

    public static DepositHistoryOperationDto MapToDto(this DepositHistoryOperation operation)
        => new DepositHistoryOperationDto(
            operation.Id.Value,
            operation.AccountId.Value,
            operation.AccountNumber.Value,
            operation.Amount.Value,
            operation.OccurredAt);

    public static WithdrawHistoryOperationDto MapToDto(this WithdrawHistoryOperation operation)
        => new WithdrawHistoryOperationDto(
            operation.Id.Value,
            operation.AccountId.Value,
            operation.AccountNumber.Value,
            operation.Amount.Value,
            operation.OccurredAt);

    public static InvoiceIssuedHistoryOperationDto MapToDto(this InvoiceIssuedHistoryOperation operation)
        => new InvoiceIssuedHistoryOperationDto(
            operation.Id.Value,
            operation.AccountId.Value,
            operation.AccountNumber.Value,
            operation.InvoiceId.Value,
            operation.OccurredAt);

    public static InvoicePaymentReceivedHistoryOperationDto MapToDto(
        this InvoicePaymentReceivedHistoryOperation operation)
        => new InvoicePaymentReceivedHistoryOperationDto(
            operation.Id.Value,
            operation.AccountId.Value,
            operation.AccountNumber.Value,
            operation.Amount.Value,
            operation.InvoiceId.Value,
            operation.OccurredAt);

    public static InvoicePaymentSentHistoryOperationDto MapToDto(
        this InvoicePaymentSentHistoryOperation operation)
        => new InvoicePaymentSentHistoryOperationDto(
            operation.Id.Value,
            operation.AccountId.Value,
            operation.AccountNumber.Value,
            operation.Amount.Value,
            operation.InvoiceId.Value,
            operation.OccurredAt);

    public static InvoiceReceivedHistoryOperationDto MapToDto(this InvoiceReceivedHistoryOperation operation)
        => new InvoiceReceivedHistoryOperationDto(
            operation.Id.Value,
            operation.AccountId.Value,
            operation.AccountNumber.Value,
            operation.InvoiceId.Value,
            operation.OccurredAt);

    public static InvoiceRevokedHistoryOperationDto MapToDto(this InvoiceRevokedHistoryOperation operation)
        => new InvoiceRevokedHistoryOperationDto(
            operation.Id.Value,
            operation.AccountId.Value,
            operation.AccountNumber.Value,
            operation.InvoiceId.Value,
            operation.OccurredAt);
}