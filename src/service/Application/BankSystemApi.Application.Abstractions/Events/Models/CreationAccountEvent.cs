namespace BankSystemApi.Application.Abstractions.Events.Models;

public sealed record CreationAccountEvent(long AccountId, long UserId, CreationAccountType AccountType);