using LogiFlow.Domain.Enums;

namespace LogiFlow.Api.Realtime.Contracts;

public sealed record DeliveryLogEventCreated(
    Guid EventId,
    Guid DeliveryId,
    DeliveryEventType Type,
    DeliveryState? FromState,
    DeliveryState? ToState,
    string Message,
    DateTimeOffset CreatedAt
);