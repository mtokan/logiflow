using LogiFlow.Domain.Enums;

namespace LogiFlow.Api.Contracts;

public sealed record TrafficSegmentResponse(
    int FromRoutePointIndex,
    int ToRoutePointIndex,
    TrafficLevel Level,
    double SpeedMultiplier
);