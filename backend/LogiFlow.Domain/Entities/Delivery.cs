using LogiFlow.Domain.Enums;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Domain.Entities;

public sealed class Delivery
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Code { get; init; }
    public required GeoPoint Origin { get; init; }
    public required GeoPoint Destination { get; init; }
    public GeoPoint? CurrentPosition { get; private set; }
    public DeliveryState State { get; private set; } = DeliveryState.Planned;
    public Guid? AssignedVehicleId { get; private set; }
    public Guid? RouteId { get; private set; }
    public TimeSpan? Eta { get; private set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public void AssignVehicle(Guid vehicleId)
    {
        AssignedVehicleId = vehicleId;
        Touch();
    }

    public void AttachRoute(Guid routeId)
    {
        RouteId = routeId;
        Touch();
    }

    public void UpdateState(DeliveryState state)
    {
        State = state;
        Touch();
    }

    public void UpdatePosition(GeoPoint position)
    {
        CurrentPosition = position;
        Touch();
    }

    public void UpdateEta(TimeSpan eta)
    {
        Eta = eta;
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}