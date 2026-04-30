using System.Collections.Concurrent;
using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;

namespace LogiFlow.Infrastructure.Persistence;

public sealed class InMemoryDeliveryEventRepository : IDeliveryEventRepository
{
    private readonly ConcurrentDictionary<Guid, List<DeliveryEvent>> _eventsByDeliveryId = new();

    public Task AddAsync(DeliveryEvent deliveryEvent, CancellationToken cancellationToken = default)
    {
        var events = _eventsByDeliveryId.GetOrAdd(
            deliveryEvent.DeliveryId, _ => []);

        lock (events)
        {
            events.Add(deliveryEvent);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<DeliveryEvent>> GetByDeliveryIdAsync(Guid deliveryId,
        CancellationToken cancellationToken = default)
    {
        if (!_eventsByDeliveryId.TryGetValue(deliveryId, out var events))
            return Task.FromResult<IReadOnlyList<DeliveryEvent>>([]);

        lock (events)
        {
            IReadOnlyList<DeliveryEvent> result = events
                .OrderBy(e => e.CreatedAt)
                .ToList();

            return Task.FromResult(result);
        }
    }
}