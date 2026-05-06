using LogiFlow.Domain.Enums;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Application.Tracking;

public record VehiclePositionUpdated(
    Guid DeliveryId,
    Guid VehicleId,
    GeoPoint Position,
    double SpeedMetersPerSecond,
    double EtaSeconds,
    double ProgressPercent,
    DeliveryState State,
    DateTimeOffset Timestamp
);