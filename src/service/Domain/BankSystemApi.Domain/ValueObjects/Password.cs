namespace BankSystemApi.Domain.ValueObjects;

public sealed record Password
{
    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Password cannot be null or whitespace.");
        }

        Value = value;
    }

    public string Value { get; }
}