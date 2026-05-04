using LogiFlow.Domain.Enums;

namespace LogiFlow.Domain.ValueObjects;

public record TrafficSegment(
    int FromRoutePointIndex,
    int ToRoutePointIndex,
    TrafficLevel Level,
    double SpeedMultiplier
);