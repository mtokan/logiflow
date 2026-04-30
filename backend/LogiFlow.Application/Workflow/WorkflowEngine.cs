using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Deliveries;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.Workflow;

public class WorkflowEngine(IDeliveryRepository deliveryRepository, IDeliveryEventRepository eventRepository)
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

        delivery.UpdateState(targetState);

        var deliveryEvent = new DeliveryEvent
        {
            DeliveryId = delivery.Id,
            Type = DeliveryEventType.StateChanged,
            FromState = currentState,
            ToState = targetState,
            Message = $"Delivery state changed from {currentState} to {targetState}."
        };

        await deliveryRepository.UpdateAsync(delivery, cancellationToken);
        await eventRepository.AddAsync(deliveryEvent, cancellationToken);

        return new WorkflowTransitionResult(delivery, deliveryEvent);
    }

    private static bool IsTransitionAllowed(DeliveryState currentState, DeliveryState targetState)

    {
        return AllowedTransitions.TryGetValue(currentState, out var allowedStates)
               && allowedStates.Contains(targetState);
    }
}