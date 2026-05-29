using IntegrationalTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationalTests.RepositoryTests;

public class BaseRepositoryTests : IAsyncDisposable
{
    protected AsyncServiceScope Scope { get; }

    public BaseRepositoryTests(WebApplicationFixture fixture)
    {
        Scope = fixture.Services.CreateAsyncScope();
    }

    public async ValueTask DisposeAsync()
    {
        await Scope.DisposeAsync();
    }
}