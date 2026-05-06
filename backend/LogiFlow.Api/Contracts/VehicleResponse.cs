using LogiFlow.Domain.Enums;

namespace LogiFlow.Api.Contracts;

public sealed record VehicleResponse(
    Guid Id,
    string Name,
    GeoPointResponse? CurrentPosition,
    double CurrentSpeedKmh,
    VehicleStatus Status,
    Guid? AssignedDeliveryId,
    bool IsActive
);