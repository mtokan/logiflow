using LogiFlow.Domain.Enums;

namespace LogiFlow.Api.Contracts;

public sealed record TransitionDeliveryRequest(
    DeliveryState TargetState
);