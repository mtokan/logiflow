using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.Workflow;

public interface IWorkflowEngine
{
    Task<WorkflowTransitionResult> TransitionAsync(
        Guid deliveryId,
        DeliveryState targetState,
        CancellationToken cancellationToken = default
    );
}