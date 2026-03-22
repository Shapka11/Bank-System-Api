namespace Bsa.Infrastructure.Persistence.Options;

public sealed class PostresOptions
{
    public required string Host { get; init; }

    public required int Port { get; init; }

    public required string Database { get; init; }

    public required string Username { get; init; }

    public required string Password { get; init; }

    public required string SslMode { get; init; }

    public required bool Pooling { get; init; }

    public string ToConnectionString()
    {
        return
            $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};SslMode={SslMode};Pooling={Pooling}";
    }
}