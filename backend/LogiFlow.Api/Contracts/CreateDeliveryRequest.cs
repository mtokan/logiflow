namespace LogiFlow.Api.Contracts;

public sealed record CreateDeliveryRequest(
    string Code,
    double OriginLatitude,
    double OriginLongitude,
    double DestinationLatitude,
    double DestinationLongitude
);