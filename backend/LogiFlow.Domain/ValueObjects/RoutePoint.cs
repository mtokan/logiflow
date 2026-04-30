namespace LogiFlow.Domain.ValueObjects;

public sealed record RoutePoint(
    double Latitude,
    double Longitude,
    int Sequence,
    double DistanceFromPreviousMeters
);