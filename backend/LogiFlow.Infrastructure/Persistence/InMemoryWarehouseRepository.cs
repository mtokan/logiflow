using System.Collections.Concurrent;
using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;

namespace LogiFlow.Infrastructure.Persistence;

public sealed class InMemoryWarehouseRepository : IWarehouseRepository
{
    private readonly ConcurrentDictionary<Guid, Warehouse> _warehouses = new();

    public Task<Warehouse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _warehouses.TryGetValue(id, out var warehouse);

        return Task.FromResult(warehouse);
    }

    public Task<IReadOnlyList<Warehouse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Warehouse> warehouses = _warehouses.Values.ToList();

        return Task.FromResult(warehouses);
    }

    public Task AddAsync(
        Warehouse warehouse,
        CancellationToken cancellationToken = default)
    {
        _warehouses[warehouse.Id] = warehouse;

        return Task.CompletedTask;
    }
}