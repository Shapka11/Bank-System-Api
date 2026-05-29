using IntegrationalTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationalTests.RepositoryTests;

public class BaseRepositoryTests : IAsyncLifetime
{
    private readonly WebApplicationFixture _fixture;

    protected AsyncServiceScope Scope { get; }

    public BaseRepositoryTests(WebApplicationFixture fixture)
    {
        _fixture = fixture;
        Scope = fixture.Services.CreateAsyncScope();
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await Scope.DisposeAsync();
    }
}