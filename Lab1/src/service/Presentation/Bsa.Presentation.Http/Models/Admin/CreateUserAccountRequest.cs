namespace Bsa.Presentation.Http.Models.Admin;

public sealed class CreateUserAccountRequest
{
    public Guid Id { get; init; }

    public required string AccountNumber { get; init; }

    public required string Password { get; init; }
}