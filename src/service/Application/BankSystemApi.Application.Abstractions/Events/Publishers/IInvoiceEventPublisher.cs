using BankSystemApi.Application.Abstractions.Events.Models;

namespace BankSystemApi.Application.Abstractions.Events.Publishers;

public interface IInvoiceEventPublisher
{
    Task Publish(IReadOnlyList<CreationInvoiceEvent> creationInvoiceEvents, CancellationToken cancellationToken);
}