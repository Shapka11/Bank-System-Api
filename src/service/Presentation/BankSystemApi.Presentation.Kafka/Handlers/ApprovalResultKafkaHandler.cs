using BankSystemApi.Application.Contracts.Invoices;
using BankSystemApi.Application.Contracts.Invoices.Operations;
using Itmo.Dev.Platform.Kafka.Consumer;
using Microsoft.Extensions.Logging;

namespace BankSystemApi.Presentation.Kafka.Handlers;

public sealed class ApprovalResultKafkaHandler
    : IKafkaConsumerHandler<ProtoApprovalResultKey, ProtoApprovalResultValue>
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<ApprovalResultKafkaHandler> _logger;

    public ApprovalResultKafkaHandler(IInvoiceService invoiceService, ILogger<ApprovalResultKafkaHandler> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<ProtoApprovalResultKey, ProtoApprovalResultValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<ProtoApprovalResultKey, ProtoApprovalResultValue> message in messages)
        {
            if (message.Value.Status == ProtoApprovalStatus.Approved)
            {
                await HandleApproveAsync(message, cancellationToken);
            }

            if (message.Value.Status == ProtoApprovalStatus.Declined)
            {
                await HandleDeclineAsync(message, cancellationToken);
            }
        }
    }

    private async Task HandleApproveAsync(
        IKafkaConsumerMessage<ProtoApprovalResultKey, ProtoApprovalResultValue> message,
        CancellationToken cancellationToken)
    {
        var approveRequest = new ApproveInvoice.Request(message.Value.InvoiceId);
        ApproveInvoice.Response approveResponse = await _invoiceService
            .ApproveAsync(approveRequest, cancellationToken);

        if (approveResponse is ApproveInvoice.Response.InvoiceNotFound invoiceNotFound)
        {
            _logger.LogError("Invoice {InvoiceId} not found", invoiceNotFound.InvoiceId);
        }

        if (approveResponse is ApproveInvoice.Response.AccountNotFound accountNotFound)
        {
            _logger.LogError("Account {AccountId} not found", accountNotFound.AccountId);
        }

        if (approveResponse is ApproveInvoice.Response.InvalidInvoiceState invalidInvoiceState)
        {
            _logger.LogError("State {State} not valid for this operation", invalidInvoiceState.State);
        }
    }

    private async Task HandleDeclineAsync(
        IKafkaConsumerMessage<ProtoApprovalResultKey, ProtoApprovalResultValue> message,
        CancellationToken cancellationToken)
    {
        var declineRequest = new DeclineInvoice.Request(message.Value.InvoiceId);
        DeclineInvoice.Response declineResponse = await _invoiceService
            .DeclineAsync(declineRequest, cancellationToken);

        if (declineResponse is DeclineInvoice.Response.InvoiceNotFound invoiceNotFound)
        {
            _logger.LogError("Invoice {InvoiceId} not found", invoiceNotFound.InvoiceId);
        }

        if (declineResponse is DeclineInvoice.Response.AccountNotFound accountNotFound)
        {
            _logger.LogError("Account {AccountId} not found", accountNotFound.AccountId);
        }

        if (declineResponse is DeclineInvoice.Response.InvalidInvoiceState invalidInvoiceState)
        {
            _logger.LogError("State {State} not valid for this operation", invalidInvoiceState.State);
        }
    }
}