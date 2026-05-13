namespace BankSystemApi.Gateway.Application.Abstractions.Users.Models;

public record BankUserModel(long Id, Guid AutorizationId, DateTimeOffset CreatedAt);