using Bsa.Gateway.Presentation.Http.Models.Invoices;
using System.ComponentModel.DataAnnotations;

namespace Bsa.Gateway.Presentation.Http.Requests.Invoices;

public sealed class GetIncomingInvoicesHttpRequest()
{
    public required Guid SessionId { get; init; }

    public string[] SenderAccountNumbers { get; init; } = [];

    public InvoiceStatusModel[] Statuses { get; init; } = [];

    [Range(minimum: 1, maximum: 1000)]
    public required int PageSize { get; init; }

    public string? PageToken { get; init; }
}