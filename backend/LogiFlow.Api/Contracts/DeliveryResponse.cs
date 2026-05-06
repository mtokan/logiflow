using LogiFlow.Domain.Enums;

namespace LogiFlow.Api.Contracts;

public sealed record DeliveryResponse(
    Guid Id,
    string Code,
    GeoPointResponse Origin,
    GeoPointResponse Destination,
    GeoPointResponse? CurrentPosition,
    DeliveryState State,
    Guid? AssignedVehicleId,
    Guid? RouteId,
    double? EtaSeconds,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);