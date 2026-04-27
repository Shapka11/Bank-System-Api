namespace BankSystemApi.Domain.ValueObjects;

public sealed record AccountNumber
{
    public AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Account number cannot be null or whitespace.");
        }

        Value = value;
    }

    public string Value { get; }
}