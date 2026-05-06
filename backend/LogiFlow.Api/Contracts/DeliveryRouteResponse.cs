namespace LogiFlow.Api.Contracts;

public sealed record DeliveryRouteResponse(
    Guid Id,
    Guid DeliveryId,
    IReadOnlyList<RoutePointResponse> Points,
    IReadOnlyList<TrafficSegmentResponse> TrafficSegments,
    double TotalDistanceMeters,
    double EstimatedDurationSeconds
);