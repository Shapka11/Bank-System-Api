namespace Bsa.Application.Providers;

public interface IDateTimeProvider
{
    DateTimeOffset Current { get; }
}