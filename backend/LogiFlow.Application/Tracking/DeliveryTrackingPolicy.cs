using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.Tracking;

public sealed class DeliveryTrackingPolicy : IDeliveryTrackingPolicy
{
    private const double ArrivingProgressThresholdPercent = 90;
    private static readonly TimeSpan ArrivingEtaThreshold = TimeSpan.FromMinutes(2);

    public DeliveryState? DetermineNextState(DeliveryState currentState, double progressPercent, TimeSpan eta,
        double currentSpeedMetersPerSecond, bool isAtFinalPoint)
    {
        var canUseEtaThreshold = currentSpeedMetersPerSecond > 0 && eta != TimeSpan.MaxValue;

        return currentState switch
        {
            DeliveryState.InTransit when progressPercent >= ArrivingProgressThresholdPercent ||
                                         (canUseEtaThreshold && eta <= ArrivingEtaThreshold)
                => DeliveryState.Arriving,

            DeliveryState.Arriving when isAtFinalPoint
                => DeliveryState.Delivered,

            _ => null
        };
    }
}