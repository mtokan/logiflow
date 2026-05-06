using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;

namespace LogiFlow.UnitTests.Workflow;

public sealed class FakeDeliveryRouteRepository : IDeliveryRouteRepository
{
    private readonly Dictionary<Guid, DeliveryRoute> _routesByDeliveryId = new();

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