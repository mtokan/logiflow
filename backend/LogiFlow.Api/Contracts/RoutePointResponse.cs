namespace LogiFlow.Api.Contracts;

public sealed record RoutePointResponse(
    double Latitude,
    double Longitude,
    int Sequence,
    double DistanceFromPreviousMeters
);