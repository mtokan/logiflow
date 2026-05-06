using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Tracking;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.Events;

public sealed class DeliveryEventService(
    IDeliveryEventRepository eventRepository,
    ITrackingUpdatePublisher trackingUpdatePublisher)
    : IDeliveryEventService
{
    public async Task AppendAsync(DeliveryEvent deliveryEvent, CancellationToken cancellationToken = default)
    {
        await eventRepository.AddAsync(deliveryEvent, cancellationToken);
        await PublishAsync(deliveryEvent, cancellationToken);
    }

    public async Task AppendManyAsync(IReadOnlyList<DeliveryEvent> deliveryEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var deliveryEvent in deliveryEvents) await AppendAsync(deliveryEvent, cancellationToken);
    }

    public Task<IReadOnlyList<DeliveryEvent>> GetTimelineAsync(Guid deliveryId,
        CancellationToken cancellationToken = default)
    {
        return eventRepository.GetByDeliveryIdAsync(deliveryId, cancellationToken);
    }

    private async Task PublishAsync(DeliveryEvent deliveryEvent, CancellationToken cancellationToken)
    {
        await trackingUpdatePublisher.PublishDeliveryEventCreatedAsync(
            new DeliveryEventCreated(
                deliveryEvent.Id,
                deliveryEvent.DeliveryId,
                deliveryEvent.Type,
                deliveryEvent.FromState,
                deliveryEvent.ToState,
                deliveryEvent.Message,
                deliveryEvent.CreatedAt),
            cancellationToken);

        if (deliveryEvent is { Type: DeliveryEventType.StateChanged, FromState: not null, ToState: not null })
            await trackingUpdatePublisher.PublishDeliveryStateChangedAsync(
                new DeliveryStateChanged(
                    deliveryEvent.DeliveryId,
                    deliveryEvent.FromState.Value,
                    deliveryEvent.ToState.Value,
                    deliveryEvent.CreatedAt),
                cancellationToken);
    }
}