using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Abstractions;

public interface IDeliveryRouteRepository
{
    Task<DeliveryRoute?> GetByDeliveryIdAsync(Guid deliveryId, CancellationToken cancellationToken = default);
    Task AddAsync(DeliveryRoute route, CancellationToken cancellationToken = default);
}