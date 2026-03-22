namespace Bsa.Application.Contracts.Models.Operations;

public sealed record AccountOperationDto(
    long Id,
    string AccountNumber,
    decimal Balance,
    string Type,
    DateTimeOffset Time);