using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.UnitTests.Workflow;

public sealed class FakeRoutingService : IRoutingService
{
    public Task<DeliveryRoute> CalculateRouteAsync(Guid deliveryId, GeoPoint origin, GeoPoint destination,
        CancellationToken cancellationToken = default)
    {
        var route = new DeliveryRoute
        {
            DeliveryId = deliveryId,
            Points =
            [
                new RoutePoint(origin.Latitude, origin.Longitude, 0, 0),
                new RoutePoint(destination.Latitude, destination.Longitude, 1, 1000)
            ],
            TotalDistanceMeters = 1000,
            EstimatedDuration = TimeSpan.FromMinutes(2)
        };

        return Task.FromResult(route);
    }
}