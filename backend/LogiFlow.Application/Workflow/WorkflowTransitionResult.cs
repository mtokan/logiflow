using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Workflow;

public sealed record WorkflowTransitionResult(
    Delivery Delivery,
    IReadOnlyList<DeliveryEvent> Events
);