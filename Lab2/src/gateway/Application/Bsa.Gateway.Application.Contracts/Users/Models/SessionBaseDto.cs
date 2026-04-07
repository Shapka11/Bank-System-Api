namespace Bsa.Gateway.Application.Contracts.Users.Models;

public abstract record SessionBaseDto(Guid Id, DateTimeOffset CreatedAt);