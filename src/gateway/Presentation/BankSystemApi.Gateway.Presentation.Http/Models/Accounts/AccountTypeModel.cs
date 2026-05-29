using System.Text.Json.Serialization;

namespace BankSystemApi.Gateway.Presentation.Http.Models.Accounts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountTypeModel
{
    Personal = 1,
    Corporate,
}