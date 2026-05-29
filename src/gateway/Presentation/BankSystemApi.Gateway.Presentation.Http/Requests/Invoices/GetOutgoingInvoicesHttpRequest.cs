using BankSystemApi.Gateway.Presentation.Http.Models.Invoices;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

public sealed class GetOutgoingInvoicesHttpRequest()
{
    public long[] ReceiverAccountIds { get; init; } = [];

    public InvoiceStatusModel[] Statuses { get; init; } = [];

    [Range(minimum: 1, maximum: 1000)]
    public required int PageSize { get; init; }

    public string? PageToken { get; init; }
}