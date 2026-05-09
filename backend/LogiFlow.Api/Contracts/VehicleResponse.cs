using LogiFlow.Domain.Enums;

namespace LogiFlow.Api.Contracts;

public sealed record VehicleResponse(
    Guid Id,
    string Name,
    VehicleStatus Status,
    Guid? AssignedDeliveryId,
    bool IsActive
);