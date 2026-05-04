using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Domain.Entities;

public class DeliveryRoute
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid DeliveryId { get; init; }
    public required IReadOnlyList<RoutePoint> Points { get; init; }
    public IReadOnlyList<TrafficSegment> TrafficSegments { get; init; } = [];
    public double TotalDistanceMeters { get; init; }
    public TimeSpan EstimatedDuration { get; init; }
}