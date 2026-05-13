namespace BankSystemApi.Application.Contracts.Users.Models;

public sealed record UserDto(long Id, Guid AutorizationId, DateTimeOffset CreatedAt);