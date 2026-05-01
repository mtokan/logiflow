using System.Collections.Concurrent;
using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;

namespace LogiFlow.Infrastructure.Persistence;

public class InMemoryDeliveryRouteRepository : IDeliveryRouteRepository
{
    private readonly ConcurrentDictionary<Guid, DeliveryRoute> _routesByDeliveryId = new();

    public Task<DeliveryRoute?> GetByDeliveryIdAsync(Guid deliveryId, CancellationToken cancellationToken = default)
    {
        _routesByDeliveryId.TryGetValue(deliveryId, out var route);
        return Task.FromResult(route);
    }

    public Task AddAsync(DeliveryRoute route, CancellationToken cancellationToken = default)
    {
        _routesByDeliveryId[route.DeliveryId] = route;
        return Task.CompletedTask;
    }
}