using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.Tracking;

public record DeliveryStateChanged(
    Guid DeliveryId,
    DeliveryState FromState,
    DeliveryState ToState,
    DateTimeOffset Timestamp
);