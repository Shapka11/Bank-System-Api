using System.Text.Json.Serialization;

namespace BankSystemApi.Gateway.Presentation.Http.Models.Invoices;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InvoiceStatusModel
{
    Created = 1,
    Paid,
    Revoked,
    Approved,
    Declined,
}