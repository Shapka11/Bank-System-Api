using BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct GetOutgoingInvoicesRequest(
    Guid UserId,
    IEnumerable<Guid> ReceiverAccountIds,
    IEnumerable<InvoiceStatusDto> Statuses,
    int PageSize,
    string? PageToken);