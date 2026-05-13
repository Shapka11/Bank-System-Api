namespace BankSystemApi.Application.Providers;

public sealed class GuidProvider : IGuidProvider
{
    public Guid NewGuid() => Guid.NewGuid();
}