using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Deliveries;
using LogiFlow.Application.Tracking;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.Workflow;

public class WorkflowEngine(
    IDeliveryRepository deliveryRepository,
    IDeliveryEventRepository eventRepository,
    IRoutingService routingService,
    ITrackingUpdatePublisher trackingUpdatePublisher,
    IDeliveryRouteRepository routeRepository)
    : IWorkflowEngine
{
    private static readonly IReadOnlyDictionary<DeliveryState, DeliveryState[]> AllowedTransitions =
        new Dictionary<DeliveryState, DeliveryState[]>
        {
            [DeliveryState.Planned] = [DeliveryState.Assigned],
            [DeliveryState.Assigned] = [DeliveryState.InTransit],
            [DeliveryState.InTransit] = [DeliveryState.Arriving],
            [DeliveryState.Arriving] = [DeliveryState.Delivered],
            [DeliveryState.Delivered] = [DeliveryState.Closed],
            [DeliveryState.Closed] = []
        };

    public async Task<WorkflowTransitionResult> TransitionAsync(Guid deliveryId, DeliveryState targetState,
        CancellationToken cancellationToken = default)
    {
        var delivery = await deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);

        if (delivery is null) throw new DeliveryNotFoundException(deliveryId);

        var currentState = delivery.State;

        if (!IsTransitionAllowed(currentState, targetState))
            throw new InvalidWorkflowTransitionException(currentState, targetState);

        var events = new List<DeliveryEvent>();
        delivery.UpdateState(targetState);

        events.Add(new DeliveryEvent
        {
            DeliveryId = delivery.Id,
            Type = DeliveryEventType.StateChanged,
            FromState = currentState,
            ToState = targetState,
            Message = $"Delivery state changed from {currentState} to {targetState}."
        });

        switch (currentState, targetState)
        {
            case (DeliveryState.Planned, DeliveryState.Assigned):
                var route = await routingService.CalculateRouteAsync(delivery.Id, delivery.Origin, delivery.Destination,
                    cancellationToken);
                await routeRepository.AddAsync(route, cancellationToken);
                delivery.AttachRoute(route.Id);

                events.Add(new DeliveryEvent
                {
                    DeliveryId = delivery.Id,
                    Type = DeliveryEventType.RouteGenerated,
                    Message = $"Route generated with {route.Points.Count} points " +
                              $"and total distance {route.TotalDistanceMeters:F0} meters."
                });
                break;
        }

        await deliveryRepository.UpdateAsync(delivery, cancellationToken);

        foreach (var deliveryEvent in events) await eventRepository.AddAsync(deliveryEvent, cancellationToken);

        foreach (var deliveryEvent in events)
        {
            await trackingUpdatePublisher.PublishDeliveryEventCreatedAsync(new DeliveryEventCreated(
                deliveryEvent.Id,
                deliveryEvent.DeliveryId,
                deliveryEvent.Type,
                deliveryEvent.FromState,
                deliveryEvent.ToState,
                deliveryEvent.Message,
                deliveryEvent.CreatedAt
            ), cancellationToken);

            if (deliveryEvent is { Type: DeliveryEventType.StateChanged, FromState: not null, ToState: not null })
                await trackingUpdatePublisher.PublishDeliveryStateChangedAsync(new DeliveryStateChanged(
                    deliveryEvent.DeliveryId,
                    deliveryEvent.FromState.Value,
                    deliveryEvent.ToState.Value,
                    deliveryEvent.CreatedAt
                ), cancellationToken);
        }

        return new WorkflowTransitionResult(delivery, events);
    }

    private static bool IsTransitionAllowed(DeliveryState currentState, DeliveryState targetState)

    {
        return AllowedTransitions.TryGetValue(currentState, out var allowedStates)
               && allowedStates.Contains(targetState);
    }
}