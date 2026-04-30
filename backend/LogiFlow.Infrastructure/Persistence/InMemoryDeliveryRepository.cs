using System.Collections.Concurrent;
using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;

namespace LogiFlow.Infrastructure.Persistence;

public sealed class InMemoryDeliveryRepository : IDeliveryRepository
{
    private readonly ConcurrentDictionary<Guid, Delivery> _deliveries = new();

    public Task<Delivery?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _deliveries.TryGetValue(id, out var delivery);
        return Task.FromResult(delivery);
    }

    public Task<IReadOnlyList<Delivery>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Delivery> deliveries = _deliveries.Values.ToList();
        return Task.FromResult(deliveries);
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