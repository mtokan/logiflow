using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.Abstractions;

public interface IDeliveryTrackingPolicy
{
    DeliveryState? DetermineNextState(
        DeliveryState currentState,
        double progressPercent,
        TimeSpan eta,
        double currentSpeedMetersPerSecond,
        bool isAtFinalPoint);
}