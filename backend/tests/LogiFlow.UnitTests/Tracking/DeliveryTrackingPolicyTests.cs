using LogiFlow.Application.Tracking;
using LogiFlow.Domain.Enums;

namespace LogiFlow.UnitTests.Tracking;

public sealed class DeliveryTrackingPolicyTests
{
    private readonly DeliveryTrackingPolicy _policy = new();

    [Fact]
    public void DetermineNextState_WhenInTransitAndProgressIsAtLeastNinety_ReturnsArriving()
    {
        var nextState = _policy.DetermineNextState(DeliveryState.InTransit, 90, TimeSpan.FromMinutes(10), 10, false);
        Assert.Equal(DeliveryState.Arriving, nextState);
    }

    [Fact]
    public void DetermineNextState_WhenInTransitAndEtaIsLessThanThreshold_ReturnsArriving()
    {
        var nextState = _policy.DetermineNextState(DeliveryState.InTransit, 50, TimeSpan.FromMinutes(1), 10, false);
        Assert.Equal(DeliveryState.Arriving, nextState);
    }

    [Fact]
    public void DetermineNextState_WhenSpeedIsZero_DoesNotUseEtaThreshold()
    {
        var nextState = _policy.DetermineNextState(DeliveryState.InTransit, 50, TimeSpan.Zero, 0, false);
        Assert.Null(nextState);
    }

    [Fact]
    public void DetermineNextState_WhenArrivingAndAtFinalPoint_ReturnsDelivered()
    {
        var nextState = _policy.DetermineNextState(DeliveryState.Arriving, 100, TimeSpan.Zero, 0, true);
        Assert.Equal(DeliveryState.Delivered, nextState);
    }

    [Fact]
    public void DetermineNextState_WhenCreated_ReturnsNull()
    {
        var nextState = _policy.DetermineNextState(DeliveryState.Created, 100, TimeSpan.Zero, 10, true);
        Assert.Null(nextState);
    }
}