using Bsa.Gateway.Application.Contracts.Invoices.Models;

namespace Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct GetIncomingInvoicesRequest(
    Guid SessionId,
    IEnumerable<string> SenderAccountNumbers,
    IEnumerable<InvoiceStatusDto> Statuses,
    int PageSize,
    string? PageToken);