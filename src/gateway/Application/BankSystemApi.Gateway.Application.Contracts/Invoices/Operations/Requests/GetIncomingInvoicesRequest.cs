using BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct GetIncomingInvoicesRequest(
    Guid UserId,
    IEnumerable<long> SenderAccountIds,
    IEnumerable<InvoiceStatusDto> Statuses,
    int PageSize,
    string? PageToken);