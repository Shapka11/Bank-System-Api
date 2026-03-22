namespace Bsa.Presentation.Http.Models.User;

public sealed class DepositRequest
{
    public required Guid Id { get; init; }

    public required decimal Amount { get; init; }
}