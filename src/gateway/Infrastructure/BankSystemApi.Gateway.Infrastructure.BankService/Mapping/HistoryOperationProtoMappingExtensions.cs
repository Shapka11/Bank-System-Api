using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models;
using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;
using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Invoices;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Mapping;

public static class HistoryOperationProtoMappingExtensions
{
    public static BankHistoryOperationModel MapToModel(this ProtoHistoryOperation proto)
    {
        return proto.OperationDataCase switch
        {
            ProtoHistoryOperation.OperationDataOneofCase.None => throw new InvalidOperationException(
                "Operation type is not set"),
            ProtoHistoryOperation.OperationDataOneofCase.CheckBalance => MapToCheckBalanceModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.CreateAccount => MapToCreateAccountModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.Deposit => MapToDepositModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.Withdraw => MapToWithdrawModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.InvoiceIssued => MapToInvoiceIssuedModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.InvoiceReceived => MapToInvoiceReceivedModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.InvoiceRevoked => MapToInvoiceRevokedModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.InvoicePaymentReceived =>
                MapToInvoicePaymentReceivedModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.InvoicePaymentSent => MapToInvoicePaymentSentModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.InvoiceApproved => MapToInvoiceApprovedModel(proto),
            ProtoHistoryOperation.OperationDataOneofCase.InvoiceDeclined => MapToInvoiceDeclinedModel(proto),
            _ => throw new ArgumentOutOfRangeException(nameof(proto), "Unknown operation type"),
        };
    }

    private static CreateAccountBankHistoryOperationModel MapToCreateAccountModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.OccurredAt.ToDateTimeOffset());

    private static CheckBalanceBankHistoryOperationModel MapToCheckBalanceModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.CheckBalance.Balance.DecimalValue,
            proto.OccurredAt.ToDateTimeOffset());

    private static DepositBankHistoryOperationModel MapToDepositModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.Deposit.Amount.DecimalValue,
            proto.OccurredAt.ToDateTimeOffset());

    private static WithdrawBankHistoryOperationModel MapToWithdrawModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.Withdraw.Amount.DecimalValue,
            proto.OccurredAt.ToDateTimeOffset());

    private static InvoiceIssuedBankHistoryOperationModel MapToInvoiceIssuedModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.InvoiceIssued.InvoiceId,
            proto.OccurredAt.ToDateTimeOffset());

    private static InvoiceReceivedBankHistoryOperationModel MapToInvoiceReceivedModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.InvoiceReceived.InvoiceId,
            proto.OccurredAt.ToDateTimeOffset());

    private static InvoiceRevokedBankHistoryOperationModel MapToInvoiceRevokedModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.InvoiceRevoked.InvoiceId,
            proto.OccurredAt.ToDateTimeOffset());

    private static InvoicePaymentReceivedBankHistoryOperationModel MapToInvoicePaymentReceivedModel(
        ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.InvoicePaymentReceived.Amount.DecimalValue,
            proto.InvoicePaymentReceived.InvoiceId,
            proto.OccurredAt.ToDateTimeOffset());

    private static InvoicePaymentSentBankHistoryOperationModel MapToInvoicePaymentSentModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.InvoicePaymentSent.Amount.DecimalValue,
            proto.InvoicePaymentSent.InvoiceId,
            proto.OccurredAt.ToDateTimeOffset());

    private static InvoiceApprovedBankHistoryOperationModel MapToInvoiceApprovedModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.InvoiceRevoked.InvoiceId,
            proto.OccurredAt.ToDateTimeOffset());

    private static InvoiceDeclinedBankHistoryOperationModel MapToInvoiceDeclinedModel(ProtoHistoryOperation proto)
        => new(
            proto.Id,
            proto.AccountId,
            proto.InvoiceRevoked.InvoiceId,
            proto.OccurredAt.ToDateTimeOffset());
}