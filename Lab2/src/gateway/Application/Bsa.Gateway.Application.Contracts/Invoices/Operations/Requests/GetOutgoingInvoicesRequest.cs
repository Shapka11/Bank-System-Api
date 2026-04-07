using Bsa.Gateway.Application.Contracts.Invoices.Models;

namespace Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;

public readonly record struct GetOutgoingInvoicesRequest(
    Guid SessionId,
    IEnumerable<string> ReceiverAccountNumbers,
    IEnumerable<InvoiceStatusDto> Statuses,
    int PageSize,
    string? PageToken);