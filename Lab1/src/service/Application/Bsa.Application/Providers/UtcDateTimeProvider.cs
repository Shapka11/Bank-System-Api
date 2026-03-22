namespace Bsa.Application.Providers;

public sealed class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Current => DateTimeOffset.UtcNow;
}