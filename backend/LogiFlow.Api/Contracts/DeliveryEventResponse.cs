using LogiFlow.Domain.Enums;

namespace LogiFlow.Api.Contracts;

public sealed record DeliveryEventResponse(
    Guid Id,
    Guid DeliveryId,
    DeliveryEventType Type,
    DeliveryState? FromState,
    DeliveryState? ToState,
    string Message,
    DateTimeOffset CreatedAt
);