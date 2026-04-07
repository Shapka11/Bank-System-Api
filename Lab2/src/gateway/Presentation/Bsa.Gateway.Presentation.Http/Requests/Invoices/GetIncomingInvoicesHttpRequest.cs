using Bsa.Gateway.Presentation.Http.Models.Invoices;
using System.ComponentModel.DataAnnotations;

namespace Bsa.Gateway.Presentation.Http.Requests.Invoices;

public readonly record struct GetIncomingInvoicesHttpRequest()
{
    public required Guid SessionId { get; init; } = Guid.Empty;

    public string[] SenderAccountNumbers { get; init; } = [];

    public InvoiceStatusModel[] Statuses { get; init; } = [];

    [Range(minimum: 1, maximum: 1000)]
    public int PageSize { get; init; } = 0;

    public string? PageToken { get; init; } = null;
}