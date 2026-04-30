using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.Workflow;

public sealed class InvalidWorkflowTransitionException(DeliveryState currentState, DeliveryState targetState)
    : Exception($"Invalid transition from {currentState} to {targetState}")
{
    public DeliveryState CurrentState { get; } = currentState;
    public DeliveryState TargetState { get; } = targetState;
}