namespace Bsa.Gateway.Application.Abstractions.Users.Models;

public abstract record BankSessionBaseModel(Guid Id, DateTimeOffset CreatedAt);