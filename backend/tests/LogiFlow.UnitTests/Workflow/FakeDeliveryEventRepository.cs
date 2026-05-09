using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;

namespace LogiFlow.UnitTests.Workflow;

public sealed class FakeDeliveryEventRepository : IDeliveryEventRepository
{
    private readonly List<DeliveryEvent> _events = [];

    public Task AddAsync(DeliveryEvent deliveryEvent, CancellationToken cancellationToken = default)
    {
        _events.Add(deliveryEvent);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<DeliveryEvent>> GetByDeliveryIdAsync(Guid deliveryId,
        CancellationToken cancellationToken = default)
    {
        var events = _events
            .Where(deliveryEvent => deliveryEvent.DeliveryId == deliveryId)
            .OrderBy(deliveryEvent => deliveryEvent.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<DeliveryEvent>>(events);
    }
}