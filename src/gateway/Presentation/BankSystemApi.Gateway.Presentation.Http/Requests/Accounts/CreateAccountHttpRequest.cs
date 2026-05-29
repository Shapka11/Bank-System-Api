using BankSystemApi.Gateway.Presentation.Http.Attributes;
using BankSystemApi.Gateway.Presentation.Http.Models.Accounts;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Gateway.Presentation.Http.Requests.Accounts;

public sealed class CreateAccountHttpRequest
{
    [NotWhiteSpace]
    public required string AccountNumber { get; init; }

    [MinLength(4, ErrorMessage = "Password must have at least 4 characters")]
    public required string Password { get; init; }

    public required AccountTypeModel AccountType { get; init; }
}