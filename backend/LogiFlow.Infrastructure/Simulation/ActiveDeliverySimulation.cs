using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Infrastructure.Simulation;

public sealed class ActiveDeliverySimulation
{
    public required Guid DeliveryId { get; init; }
    public required Guid RouteId { get; init; }
    public int CurrentRoutePointIndex { get; set; }
    public double ProgressToNextPoint { get; set; }
    public required GeoPoint CurrentPosition { get; set; }
    public double CurrentSpeedMetersPerSecond { get; set; } = 12.5;
    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastUpdateAt { get; set; } = DateTimeOffset.UtcNow;
}