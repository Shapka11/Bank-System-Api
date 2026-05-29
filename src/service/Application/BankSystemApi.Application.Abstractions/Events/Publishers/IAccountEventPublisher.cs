using BankSystemApi.Application.Abstractions.Events.Models;

namespace BankSystemApi.Application.Abstractions.Events.Publishers;

public interface IAccountEventPublisher
{
    Task Publish(IReadOnlyList<CreationAccountEvent> creationAccountEvents, CancellationToken cancellationToken);
}