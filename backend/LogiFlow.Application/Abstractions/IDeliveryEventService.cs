using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Abstractions;

public interface IDeliveryEventService
{
    Task AppendAsync(DeliveryEvent deliveryEvent, CancellationToken cancellationToken = default);
    Task AppendManyAsync(IReadOnlyList<DeliveryEvent> deliveryEvents, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryEvent>> GetTimelineAsync(Guid deliveryId, CancellationToken cancellationToken = default);
}