namespace Bsa.Presentation.Http.Models.User;

public sealed class WithdrawRequest
{
    public required Guid Id { get; init; }

    public required decimal Amount { get; init; }
}