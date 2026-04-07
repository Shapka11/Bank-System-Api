namespace Bsa.Gateway.Presentation.Http.Models.Sessions;

public sealed record AdminSessionModel(Guid Id, DateTimeOffset CreatedAt) : SessionBaseModel(Id, CreatedAt);