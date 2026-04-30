using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Abstractions;

public interface IDeliveryEventRepository
{
    Task AddAsync(DeliveryEvent deliveryEvent, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeliveryEvent>> GetByDeliveryIdAsync(Guid deliveryId,
        CancellationToken cancellationToken = default);
}