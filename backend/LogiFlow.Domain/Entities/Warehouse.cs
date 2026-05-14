using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Domain.Entities;

public sealed class Warehouse
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required GeoPoint Location { get; init; }
}