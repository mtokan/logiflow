using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Abstractions;

public interface IDeliveryRepository
{
    Task<Delivery?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Delivery delivery, CancellationToken cancellationToken = default);
    Task UpdateAsync(Delivery delivery, CancellationToken cancellationToken = default);
}