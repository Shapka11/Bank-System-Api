namespace BankSystemApi.Domain.Users;

public sealed record User(UserId Id, Guid AuthorizationId, DateTimeOffset CreatedAt);