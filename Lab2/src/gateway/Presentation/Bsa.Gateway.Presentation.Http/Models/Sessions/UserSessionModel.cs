namespace Bsa.Gateway.Presentation.Http.Models.Sessions;

public sealed record UserSessionModel(
    Guid Id,
    long AccountId,
    DateTimeOffset CreatedAt) : SessionBaseModel(Id, CreatedAt);