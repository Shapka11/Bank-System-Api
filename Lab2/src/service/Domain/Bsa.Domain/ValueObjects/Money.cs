namespace Bsa.Domain.ValueObjects;

public sealed record Money
{
    public Money(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Money cannot be negative");
        }

        Value = value;
    }

    public decimal Value { get; }

    public static Money Zero => new Money(0);

    public static Money operator +(Money lhs, Money rhs) => new Money(lhs.Value + rhs.Value);

    public static Money operator -(Money lhs, Money rhs) => new Money(lhs.Value - rhs.Value);

    public static bool operator >(Money lhs, Money rhs) => lhs.Value > rhs.Value;

    public static bool operator <(Money lhs, Money rhs) => lhs.Value < rhs.Value;
}