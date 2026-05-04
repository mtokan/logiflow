using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.Tracking;

public record DeliveryEventCreated(
    Guid EventId,
    Guid DeliveryId,
    DeliveryEventType Type,
    DeliveryState? FromState,
    DeliveryState? ToState,
    string Message,
    DateTimeOffset CreatedAt
);