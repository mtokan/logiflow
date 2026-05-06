using LogiFlow.Application.Eta;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.UnitTests.Eta;

public sealed class EtaServiceTests
{
    private readonly EtaService _etaService = new();

    [Fact]
    public void CalculateProgressPercent_WhenAtBeginning_ReturnsZero()
    {
        var route = CreateRoute();
        var progress = _etaService.CalculateProgressPercent(route, 0, 0);
        Assert.Equal(0, progress);
    }

    [Fact]
    public void CalculateProgressPercent_WhenHalfwayThroughRoute_ReturnsFifty()
    {
        var route = CreateRoute();
        var progress = _etaService.CalculateProgressPercent(route, 1, 0);
        Assert.Equal(50, progress);
    }

    [Fact]
    public void CalculateEta_UsesTrafficMultiplierPerSegment()
    {
        var route = CreateRoute();
        var eta = _etaService.CalculateEta(route, 0, 0, 10);
        Assert.Equal(TimeSpan.FromSeconds(30), eta);
    }

    [Fact]
    public void CalculateEta_WhenAtFinalPoint_ReturnsZero()
    {
        var route = CreateRoute();
        var eta = _etaService.CalculateEta(route, 2, 0, 10);
        Assert.Equal(TimeSpan.Zero, eta);
    }

    private static DeliveryRoute CreateRoute()
    {
        return new DeliveryRoute
        {
            DeliveryId = Guid.NewGuid(),
            Points =
            [
                new RoutePoint(52.0, 13.0, 0, 0),
                new RoutePoint(52.1, 13.1, 1, 100),
                new RoutePoint(52.2, 13.2, 2, 100)
            ],
            TrafficSegments =
            [
                new TrafficSegment(0, 1, TrafficLevel.Low, 1.0),
                new TrafficSegment(1, 2, TrafficLevel.Medium, 0.5)
            ],
            TotalDistanceMeters = 200,
            EstimatedDuration = TimeSpan.FromSeconds(30)
        };
    }
}