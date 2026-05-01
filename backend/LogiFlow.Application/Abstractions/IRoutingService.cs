using LogiFlow.Domain.Entities;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Application.Abstractions;

public interface IRoutingService
{
    Task<DeliveryRoute> CalculateRouteAsync(Guid deliveryId, GeoPoint origin, GeoPoint destination,
        CancellationToken cancellationToken = default);
}