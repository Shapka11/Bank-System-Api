using BankSystemApi.Application.Contracts.HistoryOperations.Models;
using BankSystemApi.Application.Contracts.HistoryOperations.Models.Accounts;
using BankSystemApi.Application.Contracts.HistoryOperations.Models.Invoices;
using Google.Protobuf.WellKnownTypes;
using Google.Type;

namespace BankSystemApi.Presentation.Grpc.Mapping.HistoryOperations;

public static class HistoryOperationProtoMappingExtensions
{
    public static ProtoHistoryOperation MapToProto(this HistoryOperationDto dto)
    {
        var proto = new ProtoHistoryOperation
        {
            Id = dto.Id,
            AccountId = dto.AccountId,
            OccurredAt = dto.OccurredAt.ToTimestamp(),
        };

        switch (dto)
        {
            case CheckBalanceHistoryOperationDto d: proto.CheckBalance = d.MapToData(); break;
            case CreateAccountHistoryOperationDto d: proto.CreateAccount = d.MapToData(); break;
            case DepositHistoryOperationDto d: proto.Deposit = d.MapToData(); break;
            case WithdrawHistoryOperationDto d: proto.Withdraw = d.MapToData(); break;
            case InvoiceIssuedHistoryOperationDto d: proto.InvoiceIssued = d.MapToData(); break;
            case InvoiceReceivedHistoryOperationDto d: proto.InvoiceReceived = d.MapToData(); break;
            case InvoiceRevokedHistoryOperationDto d: proto.InvoiceRevoked = d.MapToData(); break;
            case InvoicePaymentReceivedHistoryOperationDto d: proto.InvoicePaymentReceived = d.MapToData(); break;
            case InvoicePaymentSentHistoryOperationDto d: proto.InvoicePaymentSent = d.MapToData(); break;
            default: throw new NotSupportedException($"Type {dto.GetType().Name} is not supported");
        }

        return proto;
    }

    public static ProtoHistoryOperation.Types.CheckBalanceData MapToData(this CheckBalanceHistoryOperationDto dto)
        => new() { Balance = new Money { DecimalValue = dto.Balance } };

    public static ProtoHistoryOperation.Types.CreateAccountData MapToData(this CreateAccountHistoryOperationDto dto)
        => new();

    public static ProtoHistoryOperation.Types.DepositData MapToData(this DepositHistoryOperationDto dto)
        => new() { Amount = new Money { DecimalValue = dto.Amount } };

    public static ProtoHistoryOperation.Types.WithdrawData MapToData(this WithdrawHistoryOperationDto dto)
        => new() { Amount = new Money { DecimalValue = dto.Amount } };

    public static ProtoHistoryOperation.Types.InvoiceIssuedData MapToData(this InvoiceIssuedHistoryOperationDto dto)
        => new() { InvoiceId = dto.InvoiceId };

    public static ProtoHistoryOperation.Types.InvoiceReceivedData MapToData(this InvoiceReceivedHistoryOperationDto dto)
        => new() { InvoiceId = dto.InvoiceId };

    public static ProtoHistoryOperation.Types.InvoiceRevokedData MapToData(this InvoiceRevokedHistoryOperationDto dto)
        => new() { InvoiceId = dto.InvoiceId };

    public static ProtoHistoryOperation.Types.InvoicePaymentReceivedData MapToData(
        this InvoicePaymentReceivedHistoryOperationDto dto)
        => new() { Amount = new Money { DecimalValue = dto.Amount }, InvoiceId = dto.InvoiceId };

    public static ProtoHistoryOperation.Types.InvoicePaymentSentData MapToData(
        this InvoicePaymentSentHistoryOperationDto dto)
        => new() { Amount = new Money { DecimalValue = dto.Amount }, InvoiceId = dto.InvoiceId };

    public static ProtoHistoryOperation.Types.InvoiceApprovedData MapToData(this InvoiceApprovedHistoryOperationDto dto)
        => new() { InvoiceId = dto.InvoiceId };

    public static ProtoHistoryOperation.Types.InvoiceDeclinedData MapToData(this InvoiceDeclinedHistoryOperationDto dto)
        => new() { InvoiceId = dto.InvoiceId };
}