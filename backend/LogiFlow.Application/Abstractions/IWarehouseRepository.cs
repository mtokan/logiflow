using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Abstractions;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Warehouse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Warehouse warehouse,
        CancellationToken cancellationToken = default);
}