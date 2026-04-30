using LogiFlow.Domain.Enums;

namespace LogiFlow.Domain.Entities;

public class DeliveryEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid DeliveryId { get; init; }
    public DeliveryEventType Type { get; init; }
    public DeliveryState? FromState { get; init; }
    public DeliveryState? ToState { get; init; }
    public required string Message { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}