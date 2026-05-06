using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;

namespace LogiFlow.UnitTests.Workflow;

public sealed class FakeDeliveryRepository(params Delivery[] deliveries) : IDeliveryRepository
{
    private readonly Dictionary<Guid, Delivery> _deliveries = deliveries.ToDictionary(delivery => delivery.Id);

    public Task<Delivery?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _deliveries.TryGetValue(id, out var delivery);
        return Task.FromResult(delivery);
    }

    public Task<IReadOnlyList<Delivery>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Delivery>>(_deliveries.Values.ToList());
    }

    public Task AddAsync(Delivery delivery, CancellationToken cancellationToken = default)
    {
        _deliveries[delivery.Id] = delivery;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Delivery delivery, CancellationToken cancellationToken = default)
    {
        _deliveries[delivery.Id] = delivery;
        return Task.CompletedTask;
    }
}